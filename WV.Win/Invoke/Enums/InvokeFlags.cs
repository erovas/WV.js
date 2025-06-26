namespace WV.Win.Invoke.Enums
{
    internal enum InvokeFlags : int
    {
        // Flags base
        DISPATCH_METHOD = 0x1,   // (1) Invoca un método
        DISPATCH_PROPERTYGET = 0x2,   // (2) Obtiene el valor de una propiedad
        DISPATCH_PROPERTYPUT = 0x4,   // (4) Asigna un valor a una propiedad (por valor)
        DISPATCH_PROPERTYPUTREF = 0x8,   // (8) Asigna una referencia a una propiedad (por referencia)

        // Flags adicionales (menos comunes)
        DISPATCH_CONSTRUCT = 0x4000, // (16384) Crea un objeto (constructor)
        DISPATCH_EVENT = 0x2000, // (8192) Manejo de eventos (usado en conexiones de puntos de eventos)
        DISPATCH_STDPROPGET = 0x10,   // (16) Propiedad estándar "get"
        DISPATCH_STDPROPPUT = 0x20,   // (32) Propiedad estándar "put"
        DISPATCH_STDMETHOD = 0x40,   // (64) Método estándar

        // Flags combinados/reservados
        DISPATCH_ASYNCHRONOUS = 0x1000, // (4096) Ejecución asincrónica (no ampliamente soportado)
        DISPATCH_HAS_RESULT = 0x8000  // (32768) Indica que la llamada retorna un valor (usado internamente)
    }
}