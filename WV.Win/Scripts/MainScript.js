const WIN = window;
const HONAME = '-HostObjectName-';  // será reemplazado por el UID de la ventana

//==============================================================//

//Comprobar que existe 'webview'
if (!WIN.chrome.webview)
    return;

//Comprobar que existe hostObjects
if (!WIN.chrome.webview.hostObjects)
    return;

//Comprobar que existe el objeto puente
if (!WIN.chrome.webview.hostObjects.sync[HONAME])
    return;

//==============================================================//

const WV = WIN.chrome.webview;
const HO = WV.hostObjects;
const SYNC = HO.sync[HONAME];
const ASYNC = HO[HONAME];
const EXPOSE = Object.create(null);

//==============================================================//

//Hacer que un Date() de js se convierta en DateTime() de c# cuando se le pasa a un HostObject (HO)
HO.options.shouldSerializeDates = true;

//Ignorar propiedades que NO existen en el plugin C#
HO.options.ignoreMemberNotFoundError = true;

// Pasar arrays tipados JS como array
HO.options.shouldPassTypedArraysAsArrays = true;

//Tratar de limpiar referencias de objetos C# creados (Función propia de WV2)
HO.cleanupSome();

//==============================================================//

Object.defineProperties(EXPOSE, {
    Sync: {
        get: () => SYNC
    },
    Async: {
        get: () => ASYNC
    }
});

Object.defineProperty(WIN, SYNC.Name, {
    get: () => EXPOSE
});