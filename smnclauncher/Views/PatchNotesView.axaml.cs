using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Markdown.Avalonia;
using ReactiveUI;
using smnclauncher.ViewModels;
using System.Reactive.Disposables;

namespace smnclauncher.Views
{
    public class PatchNotesView : ReactiveUserControl<PatchNotesViewModel>
    {
        private MarkdownScrollViewer PatchNotes => this.FindControl<MarkdownScrollViewer>("PatchNotes");

        public PatchNotesView()
        {
            this.InitializeComponent();
            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.PatchNotes, v => v.PatchNotes.Markdown)
                    .DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
