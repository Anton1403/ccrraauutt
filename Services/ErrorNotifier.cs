namespace crraut.Services; 

public class ErrorNotifier {
    private readonly Bugsnag.Client _bugsnagClient;

    public ErrorNotifier(Bugsnag.Client bugsnagClient) {
        _bugsnagClient = bugsnagClient;
    }

    public void Notify(Exception exception) {
        _bugsnagClient.Notify(exception);
    }
}