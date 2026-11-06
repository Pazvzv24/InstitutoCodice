using ClosedXML.Excel;
using InstitutoCodice.Negocio.Modelos;
using InstitutoCodice.Negocio.Servicios;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
// Alias: en Windows Forms ya existen tipos con estos mismos nombres
// (System.ComponentModel.IContainer, System.Drawing.Color, System.Reflection.Metadata.Document).
// Con estos alias, en este archivo siempre se usan los de QuestPDF.
using Document = QuestPDF.Fluent.Document;
using IContainer = QuestPDF.Infrastructure.IContainer;
using Color = QuestPDF.Infrastructure.Color;

namespace InstitutoCodice.App
{
    /// <summary>
    /// Datos de un informe ya calculado y filtrado por permisos, listo para exportar.
    /// </summary>
    public class DatosInforme
    {
        public string Titulo { get; set; } = string.Empty;
        public string Resumen { get; set; } = string.Empty;
        public bool PorEstudiante { get; set; }
        public List<LineaInforme> Lineas { get; set; } = new List<LineaInforme>();
        public string EmitidoPor { get; set; } = string.Empty;
        public DateTime FechaEmision { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Exporta informes a Excel (ClosedXML) y PDF (QuestPDF).
    /// Exporta solo lo que el usuario ya puede ver en pantalla: el control de acceso
    /// se aplica antes, en ServicioInformes.
    /// </summary>
    public static class ExportadorInformes
    {
        private const string Aviso = "Documento con datos personales. Uso interno del Instituto Códice (Ley 21.719).";

        static ExportadorInformes()
        {
            // Licencia gratuita para personas y empresas con ingresos bajo 1 millón USD al año
            QuestPDF.Settings.License = LicenseType.Community;
        }

        // ------------------------------------------------------------------ Excel

        public static void ExportarExcel(DatosInforme datos, string ruta)
        {
            using (var libro = new XLWorkbook())
            {
                var hoja = libro.Worksheets.Add("Informe");

                hoja.Cell(1, 1).Value = "Instituto Códice";
                hoja.Cell(1, 1).Style.Font.Bold = true;
                hoja.Cell(1, 1).Style.Font.FontSize = 14;
                hoja.Cell(2, 1).Value = datos.Titulo;
                hoja.Cell(2, 1).Style.Font.Bold = true;
                hoja.Cell(3, 1).Value = "Emitido el " + datos.FechaEmision.ToString("dd-MM-yyyy HH:mm") + " por " + datos.EmitidoPor;
                hoja.Cell(4, 1).Value = datos.Resumen;
                hoja.Range(3, 1, 4, 1).Style.Font.FontColor = XLColor.Gray;

                string[] encabezados = Encabezados(datos.PorEstudiante);
                const int filaEncabezado = 6;
                for (int c = 0; c < encabezados.Length; c++)
                    hoja.Cell(filaEncabezado, c + 1).Value = encabezados[c];

                int fila = filaEncabezado + 1;
                foreach (var linea in datos.Lineas)
                {
                    int col = 1;
                    if (datos.PorEstudiante)
                    {
                        Texto(hoja.Cell(fila, col++), linea.Asignatura);
                    }
                    else
                    {
                        Texto(hoja.Cell(fila, col++), linea.Rut);
                        Texto(hoja.Cell(fila, col++), linea.NombreEstudiante);
                    }

                    Texto(hoja.Cell(fila, col++), linea.Notas);

                    // Promedio y asistencia como números, para poder ordenar y calcular en Excel
                    var celdaPromedio = hoja.Cell(fila, col++);
                    if (linea.CantidadNotas > 0)
                    {
                        celdaPromedio.Value = (double)linea.Promedio;
                        celdaPromedio.Style.NumberFormat.Format = "0.0";
                    }
                    else
                    {
                        Texto(celdaPromedio, "-");
                    }

                    var celdaAsistencia = hoja.Cell(fila, col++);
                    if (linea.TotalClases > 0)
                    {
                        celdaAsistencia.Value = (double)linea.PorcentajeAsistencia / 100.0;
                        celdaAsistencia.Style.NumberFormat.Format = "0.0%";
                    }
                    else
                    {
                        Texto(celdaAsistencia, "-");
                    }

                    var celdaEstado = hoja.Cell(fila, col);
                    Texto(celdaEstado, linea.Estado);
                    celdaEstado.Style.Font.FontColor = ColorExcel(linea.Estado);
                    celdaEstado.Style.Font.Bold = true;

                    fila++;
                }

                // Tabla con filtros y encabezado destacado
                var rango = hoja.Range(filaEncabezado, 1, Math.Max(fila - 1, filaEncabezado), encabezados.Length);
                var tabla = rango.CreateTable("TablaInforme");
                tabla.Theme = XLTableTheme.TableStyleLight9;

                hoja.Cell(fila + 1, 1).Value = Aviso;
                hoja.Cell(fila + 1, 1).Style.Font.Italic = true;
                hoja.Cell(fila + 1, 1).Style.Font.FontColor = XLColor.Gray;

                hoja.Columns(1, encabezados.Length).AdjustToContents(filaEncabezado, fila);
                hoja.SheetView.FreezeRows(filaEncabezado);

                libro.SaveAs(ruta);
            }
        }

        /// <summary>
        /// Escribe siempre como texto: un valor que empiece con "=", "+", "-" o "@"
        /// nunca se interpreta como fórmula (evita la inyección de fórmulas).
        /// </summary>
        private static void Texto(IXLCell celda, string? valor)
        {
            celda.Value = valor ?? string.Empty;
            celda.Style.NumberFormat.Format = "@";
        }

        private static XLColor ColorExcel(string estado)
        {
            if (estado == ServicioInformes.Aprobado) return XLColor.DarkGreen;
            if (estado == ServicioInformes.Reprobado) return XLColor.Firebrick;
            if (estado == ServicioInformes.Parcial) return XLColor.DarkOrange;
            return XLColor.DimGray;
        }

        // -------------------------------------------------------------------- PDF

        public static void ExportarPdf(DatosInforme datos, string ruta)
        {
            string[] encabezados = Encabezados(datos.PorEstudiante);

            Document.Create(documento =>
            {
                documento.Page(pagina =>
                {
                    pagina.Size(PageSizes.A4.Landscape());
                    pagina.Margin(30);
                    pagina.DefaultTextStyle(estilo => estilo.FontSize(9));

                    pagina.Header().Column(columna =>
                    {
                        columna.Item().Text("Instituto Códice").FontSize(16).Bold().FontColor(Colors.Indigo.Darken2);
                        columna.Item().Text(datos.Titulo).FontSize(11).SemiBold();
                        columna.Item().Text("Emitido el " + datos.FechaEmision.ToString("dd-MM-yyyy HH:mm") + " por " + datos.EmitidoPor)
                              .FontColor(Colors.Grey.Darken1);
                        columna.Item().PaddingTop(4).Text(datos.Resumen).FontColor(Colors.Grey.Darken1);
                    });

                    pagina.Content().PaddingVertical(12).Table(tabla =>
                    {
                        tabla.ColumnsDefinition(columnas =>
                        {
                            if (datos.PorEstudiante)
                            {
                                columnas.RelativeColumn(3);   // Asignatura
                            }
                            else
                            {
                                columnas.RelativeColumn(1.5f); // RUT
                                columnas.RelativeColumn(3);    // Estudiante
                            }
                            columnas.RelativeColumn(4);   // Notas
                            columnas.RelativeColumn(1.2f); // Promedio
                            columnas.RelativeColumn(1.3f); // Asistencia
                            columnas.RelativeColumn(1.4f); // Estado
                        });

                        tabla.Header(encabezado =>
                        {
                            foreach (string titulo in encabezados)
                                encabezado.Cell().Element(CeldaEncabezado).Text(titulo).SemiBold();
                        });

                        foreach (var linea in datos.Lineas)
                        {
                            if (datos.PorEstudiante)
                            {
                                tabla.Cell().Element(Celda).Text(linea.Asignatura);
                            }
                            else
                            {
                                tabla.Cell().Element(Celda).Text(linea.Rut);
                                tabla.Cell().Element(Celda).Text(linea.NombreEstudiante);
                            }

                            tabla.Cell().Element(Celda).Text(linea.Notas);
                            tabla.Cell().Element(Celda).AlignCenter()
                                 .Text(linea.CantidadNotas > 0 ? linea.Promedio.ToString("0.0") : "-");
                            tabla.Cell().Element(Celda).AlignCenter()
                                 .Text(linea.TotalClases > 0 ? linea.PorcentajeAsistencia.ToString("0.0") + " %" : "-");
                            tabla.Cell().Element(Celda).AlignCenter()
                                 .Text(linea.Estado).SemiBold().FontColor(ColorPdf(linea.Estado));
                        }
                    });

                    pagina.Footer().Column(columna =>
                    {
                        columna.Item().Text("Aprobado: promedio ≥ 4,0 y asistencia ≥ 75 %. Parcial: faltan evaluaciones por calificar.")
                              .FontSize(8).FontColor(Colors.Grey.Darken1);
                        columna.Item().Row(fila =>
                        {
                            fila.RelativeItem().Text(Aviso).FontSize(8).Italic().FontColor(Colors.Grey.Darken1);
                            fila.ConstantItem(90).AlignRight().Text(texto =>
                            {
                                texto.DefaultTextStyle(estilo => estilo.FontSize(8));
                                texto.Span("Página ");
                                texto.CurrentPageNumber();
                                texto.Span(" de ");
                                texto.TotalPages();
                            });
                        });
                    });
                });
            }).GeneratePdf(ruta);
        }

        private static IContainer CeldaEncabezado(IContainer contenedor)
        {
            return contenedor.Background(Colors.Grey.Lighten3).PaddingVertical(5).PaddingHorizontal(4);
        }

        private static IContainer Celda(IContainer contenedor)
        {
            return contenedor.BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4);
        }

        private static Color ColorPdf(string estado)
        {
            if (estado == ServicioInformes.Aprobado) return Colors.Green.Darken2;
            if (estado == ServicioInformes.Reprobado) return Colors.Red.Darken2;
            if (estado == ServicioInformes.Parcial) return Colors.Orange.Darken2;
            return Colors.Grey.Darken1;
        }

        // ---------------------------------------------------------------- Común

        private static string[] Encabezados(bool porEstudiante)
        {
            return porEstudiante
                ? new[] { "Asignatura", "Notas", "Promedio", "% Asistencia", "Estado" }
                : new[] { "RUT", "Estudiante", "Notas", "Promedio", "% Asistencia", "Estado" };
        }
    }
}