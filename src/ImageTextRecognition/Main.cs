// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Main.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The main form.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ImageTextRecognition;

/// <summary>
/// The main form.
/// </summary>
public partial class Main : Form
{
    /// <summary>
    /// The language manager.
    /// </summary>
    private readonly ILanguageManager languageManager = new LanguageManager();

    /// <summary>
    /// The OCR reader.
    /// </summary>
    private readonly IImageOcrReader ocrReader = new ImageOcrReader();

    /// <summary>
    /// The background worker.
    /// </summary>
    private readonly BackgroundWorker worker = new();

    /// <summary>
    /// The language.
    /// </summary>
    private ILanguage? language;

    /// <summary>
    /// Gets or sets the files.
    /// </summary>
    private Dictionary<string, string> files = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Main"/> class.
    /// </summary>
    public Main()
    {
        this.InitializeComponent();
        this.InitializeCaption();
        this.InitializeLanguageManager();
        this.LoadLanguagesToCombo();
        this.InitializeBackgroundWorker();
    }

    /// <summary>
    /// Initializes the language manager.
    /// </summary>
    private void InitializeLanguageManager()
    {
        this.languageManager.SetCurrentLanguage("de-DE");
        this.languageManager.OnLanguageChanged += this.OnLanguageChanged;
        this.language = this.languageManager.GetCurrentLanguage();
    }

    /// <summary>
    /// Initializes the background worker.
    /// </summary>
    private void InitializeBackgroundWorker()
    {
        this.worker.DoWork += this.RunFileScan;
        this.worker.RunWorkerCompleted += this.FileScanCompleted;
    }

    /// <summary>
    /// Handles the file scan work.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    private void RunFileScan(object sender, DoWorkEventArgs e)
    {
        if (this.language is null)
        {
            return;
        }

        var fileNameText = this.language.GetWord("FileName");

        foreach (var (key, _) in this.files)
        {
            var text = this.ocrReader.ReadTextOnImage(key);

            this.UiThreadInvoke(
                () =>
                {
                    this.RichTextBoxResult.AppendText($"{fileNameText}: {key}");
                    this.RichTextBoxResult.AppendText(Environment.NewLine);
                    this.RichTextBoxResult.AppendText(text);
                    this.RichTextBoxResult.AppendText(Environment.NewLine);
                    this.RichTextBoxResult.AppendText(
                        "-------------------------------------------------------------------------");
                    this.RichTextBoxResult.AppendText(Environment.NewLine);
                });
        }
    }

    /// <summary>
    /// Handles the file scan completed event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    private void FileScanCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        if (this.language is null)
        {
            return;
        }

        MessageBox.Show(this.language.GetWord("FinishedText"), this.language.GetWord("FinishedTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    /// <summary>
    /// Loads the languages to the combo box.
    /// </summary>
    private void LoadLanguagesToCombo()
    {
        foreach (var localLanguage in this.languageManager.GetLanguages())
        {
            this.comboBoxLanguage.Items.Add(localLanguage.Name);
        }

        this.comboBoxLanguage.SelectedIndex = 0;
    }

    /// <summary>
    /// Handles the combo box selected event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    private void ComboBoxLanguageSelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedItem = this.comboBoxLanguage.SelectedItem.ToString();

        if (string.IsNullOrWhiteSpace(selectedItem))
        {
            return;
        }

        this.languageManager.SetCurrentLanguageFromName(selectedItem);
    }

    /// <summary>
    /// Initializes the caption.
    /// </summary>
    private void InitializeCaption()
    {
        this.Text = $@"{Application.ProductName} {Application.ProductVersion}";
    }

    /// <summary>
    /// Handles the language changed event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    private void OnLanguageChanged(object sender, EventArgs e)
    {
        this.ButtonOpenImages.Text = this.languageManager.GetCurrentLanguage().GetWord("OpenImages");
        this.language = this.languageManager.GetCurrentLanguage();
    }

    /// <summary>
    /// Handles the button click to open some images.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    private void ButtonOpenImagesClick(object sender, EventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            CheckFileExists = true,
            CheckPathExists = true,
            Multiselect = true,
            Filter = this.language?.GetWord("ImageFilter") ?? string.Empty
        };

        var result = openFileDialog.ShowDialog();

        if (result != DialogResult.OK)
        {
            return;
        }

        this.files = new Dictionary<string, string>();

        foreach (var fileName in openFileDialog.FileNames)
        {
            this.files.Add(fileName, string.Empty);
        }

        this.worker.RunWorkerAsync();
    }
}
