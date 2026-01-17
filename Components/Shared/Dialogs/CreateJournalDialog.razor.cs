using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using JournalManagementSystem.Models;
using MudBlazor.Services;

namespace JournalManagementSystem.Components.Shared.Dialogs;

public class CreateJournalDialogBase : ComponentBase
{
    [CascadingParameter]
    //public MudDialogInstance MudDialog { get; set; } = null!;

    [Inject]
    public IJSRuntime JS { get; set; } = null!;

    protected MudForm? _form;
    protected Journal _model = new();
    protected HashSet<string> _secondaryMoods = new();

    protected string EditorId { get; set; } = $"quill-editor-{Guid.NewGuid()}";

    protected string[] PrimaryMoods = new[]
    {
        "Happy", "Sad", "Anxious", "Calm", "Excited",
        "Angry", "Grateful", "Stressed", "Peaceful", "Confused"
    };

    protected string[] SecondaryMoods = new[]
    {
        "Hopeful", "Tired", "Energetic", "Lonely", "Loved",
        "Frustrated", "Inspired", "Bored", "Proud", "Ashamed"
    };

    protected string[] Tags = new[]
    {
        "Work", "Personal", "Health", "Relationships", "Goals",
        "Dreams", "Memories", "Reflection", "Gratitude", "Growth"
    };

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JS.InvokeVoidAsync("initQuill", EditorId);
        }
    }

    protected void OnSecondaryMoodsChanged(IEnumerable<string> selectedMoods)
    {
        var moodsList = selectedMoods.ToList();

        if (moodsList.Count > 2)
        {
            // Keep only the last 2 selected
            _secondaryMoods = moodsList.Skip(moodsList.Count - 2).ToHashSet();
        }
        else
        {
            _secondaryMoods = moodsList.ToHashSet();
        }

        _model.SecondaryMoods = _secondaryMoods.ToList();
    }

    protected async Task Save()
    {
        await _form!.Validate();

        if (!_form.IsValid)
            return;

        // Get the HTML content from Quill editor
        _model.DescriptionHtml = await JS.InvokeAsync<string>("getQuillHtml");
        _model.SecondaryMoods = _secondaryMoods.ToList();

        //MudDialog.Close(DialogResult.Ok(_model));
    }

    //protected void Cancel()
    //{
    //    MudDialog.Cancel();
    //}
}