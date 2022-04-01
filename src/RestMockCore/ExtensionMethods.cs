using RestMockCore.Models;

namespace RestMockCore;

public static class ExtensionMethods
{
    public static RouteTableItem Verifiable(this RouteTableItem handler)
    {
        handler.IsVerifiable = true;
        return handler;
    }
}
