const WIN = window;
const HONAME = '-HostObjectName-';

try {
    //Comprobar que existe 'webview'
    if (!WIN.chrome.webview)
        return;

    //Comprobar que existe hostObjects
    if (!WIN.chrome.webview.hostObjects)
        return;

    //Comprobar que existe el objeto puente
    if (!WIN.chrome.webview.hostObjects.sync[HONAME])
        return;

} catch (e) {
    return;
}

const NAME = '-Name-';
const WV = WIN.chrome.webview;
const HO = WV.hostObjects;
const SYNC = HO.sync[HONAME];
const ISFRAME = SYNC.IsFrame;
/*const ISFRAME = (_ => {
    try {
        return WIN.self !== WIN.top;
    } catch (e) {
        return true;
    }
})();
*/

//Hacer que un Date() de js se convierta en DateTime() de c# cuando se le pasa a un HO
HO.options.shouldSerializeDates = true;

//Ignorar propiedades que NO existen en el plugin C#
HO.options.ignoreMemberNotFoundError = true;

// Para escuchar a SendMessageAsString() y SendMessageAsJson(), de un WebView y/o Frame
(_ => {
    const name = 'message';
    const EvName = NAME + name;
    const CEvent = new CustomEvent(EvName);
    let fn = null;

    Object.defineProperty(WIN, 'on' + EvName, {
        set(value) {

            if (fn)
                WIN.removeEventListener(EvName, fn, false);

            if (typeof value === 'function') {
                fn = value;
                WIN.addEventListener(EvName, fn, false);
            }
            else
                fn = null;
        },
        get() {
            return fn;
        }
    });

    WV.addEventListener(name, e => {
        CEvent.data = e.data;
        WIN.dispatchEvent(CEvent);
    });

})();

// Webview JS Events
(_ => {

    // un iframe NO tiene JSEvents
    if (ISFRAME)
        return;

    //JSEvent = ['state', 'position', ...];
    'JSEvent'.forEach(name => {
        const EvName = NAME + name;
        const CEvent = new CustomEvent(EvName);
        let fn = null;

        Object.defineProperty(WIN, 'on' + EvName, {
            set(value) {

                if (fn)
                    WIN.removeEventListener(EvName, fn, false);

                if (typeof value === 'function') {
                    fn = value;
                    WIN.addEventListener(EvName, fn, false);
                }
                else
                    fn = null;
            },
            get() {
                return fn;
            }
        });

        const fx = x => {
            try {
                CEvent.data = JSON.parse(x);
            } catch (error) {
                CEvent.data = x;
            }

            WIN.dispatchEvent(CEvent);
        }

        SYNC.AE(name, fx, fx.toString());
    });

})();

WIN[NAME] = {
    WV,
    HO,
    SYNC,
    ASYNC: HO[HONAME],
    ISFRAME
}