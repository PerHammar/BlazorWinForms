using BlazorWinForms.Interop;

namespace WinFormsBlazor.Requests.Handlers;

public class UpdateFormTextHandler : IRequestHandler<UpdateFormText, bool>
{
    public Task<bool> HandleAsync(UpdateFormText request, CancellationToken cancellationToken = default)
    {
        FormController.Instance.UpdateFormText(request.Text);
        return Task.FromResult(true);
    }
}
