using BlazorWinForms.Interop;

namespace WinFormsBlazor.Requests.Handlers;

public class CloseSecondFormHandler : IRequestHandler<CloseSecondForm, bool>
{
    public Task<bool> HandleAsync(CloseSecondForm request, CancellationToken cancellationToken = default)
    {
        FormController.Instance.CloseSecondForm();
        return Task.FromResult(true);
    }
}
