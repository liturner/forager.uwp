using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Graphics.Printing;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Printing;

namespace Forager
{
    public class GroupInfoList<T> : List<object>
    {
        public object Key { get; set; }

        public new IEnumerator<object> GetEnumerator()
        {
            return base.GetEnumerator();
        }
    }

    namespace Tools
    {
        public class PrintHelper
        {
            #region Application Content Size Constants given in percents (normalized)

            /// <summary>
            /// The percent of app's margin width, content is set at 85% (0.85) of the area's width
            /// </summary>
            protected const double ApplicationContentMarginLeft = 0.075;

            /// <summary>
            /// The percent of app's margin height, content is set at 94% (0.94) of tha area's height
            /// </summary>
            protected const double ApplicationContentMarginTop = 0.03;

            #endregion

            /// <summary>
            /// PrintDocument is used to prepare the pages for printing.
            /// Prepare the pages to print in the handlers for the Paginate, GetPreviewPage, and AddPages events.
            /// </summary>
            protected PrintDocument printDocument;

            /// <summary>
            /// Marker interface for document source
            /// </summary>
            protected IPrintDocumentSource printDocumentSource;

            /// <summary>
            /// A list of UIElements used to store the print preview pages.  This gives easy access
            /// to any desired preview page.
            /// </summary>
            internal List<UIElement> printPreviewPages = new List<UIElement>();

            // Event callback which is called after print preview pages are generated.  Photos scenario uses this to do filtering of preview pages
            protected event EventHandler PreviewPagesCreated;

            /// <summary>
            /// First page in the printing-content series
            /// From this "virtual sized" paged content is split(text is flowing) to "printing pages"
            /// </summary>
            public Page PageToPrint;

            /// <summary>
            ///  A reference back to the scenario page used to access XAML elements on the scenario page
            /// </summary>
            protected readonly Page originalPageToPrint;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="_pageToPrint">The scenario page constructing us</param>
            public PrintHelper()
            {

            }

            /// <summary>
            /// This function registers the app for printing with Windows and sets up the necessary event handlers for the print process.
            /// </summary>
            public virtual void RegisterForPrinting()
            {
                printDocument = new PrintDocument();
                printDocumentSource = printDocument.DocumentSource;
                printDocument.Paginate += CreatePrintPreviewPages;
                printDocument.GetPreviewPage += GetPrintPreviewPage;
                printDocument.AddPages += AddPrintPages;

                PrintManager printMan = PrintManager.GetForCurrentView();
                printMan.PrintTaskRequested += PrintTaskRequested;
            }

            /// <summary>
            /// This function unregisters the app for printing with Windows.
            /// </summary>
            public virtual void UnregisterForPrinting()
            {
                if (printDocument == null)
                {
                    return;
                }

                printDocument.Paginate -= CreatePrintPreviewPages;
                printDocument.GetPreviewPage -= GetPrintPreviewPage;
                printDocument.AddPages -= AddPrintPages;

                // Remove the handler for printing initialization.
                PrintManager printMan = PrintManager.GetForCurrentView();
                printMan.PrintTaskRequested -= PrintTaskRequested;
            }

            public async Task ShowPrintUIAsync()
            {
                // Catch and print out any errors reported
                try
                {
                    await PrintManager.ShowPrintUIAsync();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }

            ///// <summary>
            ///// Method that will generate print content for the scenario
            ///// For scenarios 1-4: it will create the first page from which content will flow
            ///// Scenario 5 uses a different approach
            ///// </summary>
            ///// <param name="page">The page to print</param>
            //public virtual void PreparePrintContent(Type _templatePageType, object _dataSource)
            //{

            //    PageToPrint = System.Activator.CreateInstance(_templatePageType) as Page;
            //    PageToPrint.DataContext = _dataSource;

            //    // Add the (newly created) page to the print canvas which is part of the visual tree and force it to go
            //    // through layout so that the linked containers correctly distribute the content inside them.
            //    //PrintCanvas.Children.Add(firstPage);
            //    //PrintCanvas.InvalidateMeasure();
            //    //PrintCanvas.UpdateLayout();
            //}

            /// <summary>
            /// This is the event handler for PrintManager.PrintTaskRequested.
            /// </summary>
            /// <param name="sender">PrintManager</param>
            /// <param name="e">PrintTaskRequestedEventArgs </param>
            protected virtual void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs e)
            {
                PrintTask printTask = null;
                printTask = e.Request.CreatePrintTask("Forager Recipe Print", (PrintTaskSourceRequestedHandler)(sourceRequested =>
                {
                    // Print Task event handler is invoked when the print job is completed.
                    printTask.Completed += async (s, args) =>
                    {
                        // Notify the user when the print operation fails.
                        if (args.Completion == PrintTaskCompletion.Failed)
                        {

                        }
                    };

                    sourceRequested.SetSource(printDocumentSource);
                }));
            }

            /// <summary>
            /// This is the event handler for PrintDocument.Paginate. It creates print preview pages for the app.
            /// </summary>
            /// <param name="sender">PrintDocument</param>
            /// <param name="e">Paginate Event Arguments</param>
            protected virtual void CreatePrintPreviewPages(object sender, PaginateEventArgs e)
            {
                if (PageToPrint == null)
                    throw new NullReferenceException("PageToPrint cannot be null before requeesting a Print operation");

                // Clear the cache of preview pages
                printPreviewPages.Clear();
                printPreviewPages.Add(PageToPrint);

                PrintPageDescription pageDescription = ((PrintTaskOptions)e.PrintTaskOptions).GetPageDescription(0);

                // Set "paper" width
                PageToPrint.Width = pageDescription.PageSize.Width;
                PageToPrint.Height = pageDescription.PageSize.Height;

                Grid printableArea = PageToPrint.Content as Grid;

                // Get the margins size
                // If the ImageableRect is smaller than the app provided margins use the ImageableRect
                double marginWidth = Math.Max(pageDescription.PageSize.Width - pageDescription.ImageableRect.Width, pageDescription.PageSize.Width * ApplicationContentMarginLeft * 2);
                double marginHeight = Math.Max(pageDescription.PageSize.Height - pageDescription.ImageableRect.Height, pageDescription.PageSize.Height * ApplicationContentMarginTop * 2);

                // Set-up "printable area" on the "paper"
                printableArea.Width = PageToPrint.Width - marginWidth;
                printableArea.Height = PageToPrint.Height - marginHeight;

                PageToPrint.UpdateLayout();

                if (PreviewPagesCreated != null)
                {
                    PreviewPagesCreated.Invoke(printPreviewPages, null);
                }

                PrintDocument printDoc = (PrintDocument)sender;

                // Report the number of preview pages created
                printDoc.SetPreviewPageCount(1, PreviewPageCountType.Intermediate);
            }

            /// <summary>
            /// This is the event handler for PrintDocument.GetPrintPreviewPage. It provides a specific print preview page,
            /// in the form of an UIElement, to an instance of PrintDocument. PrintDocument subsequently converts the UIElement
            /// into a page that the Windows print system can deal with.
            /// </summary>
            /// <param name="sender">PrintDocument</param>
            /// <param name="e">Arguments containing the preview requested page</param>
            protected virtual void GetPrintPreviewPage(object sender, GetPreviewPageEventArgs e)
            {
                PrintDocument printDoc = (PrintDocument)sender;
                printDoc.SetPreviewPage(e.PageNumber, printPreviewPages[e.PageNumber - 1]);
            }

            /// <summary>
            /// This is the event handler for PrintDocument.AddPages. It provides all pages to be printed, in the form of
            /// UIElements, to an instance of PrintDocument. PrintDocument subsequently converts the UIElements
            /// into a pages that the Windows print system can deal with.
            /// </summary>
            /// <param name="sender">PrintDocument</param>
            /// <param name="e">Add page event arguments containing a print task options reference</param>
            protected virtual void AddPrintPages(object sender, AddPagesEventArgs e)
            {
                // Loop over all of the preview pages and add each one to  add each page to be printied
                for (int i = 0; i < printPreviewPages.Count; i++)
                {
                    // We should have all pages ready at this point...
                    printDocument.AddPage(printPreviewPages[i]);
                }

                PrintDocument printDoc = (PrintDocument)sender;

                // Indicate that all of the print pages have been provided
                printDoc.AddPagesComplete();
            }


        }
    }
}
