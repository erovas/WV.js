const NAME = 'Window';
const WV = window.wv;
const WV_ASYNC = WV.async();
const New = WV.new;
const NewAsync = WV_ASYNC.new;
const Stringify = JSON.stringify;
const Default = {
    LocalServer: true,
    Domain: null,
    IndexPage: null,
    ErrorPage: null,
    EnableFramePlugins: false,
    EnablePlugins: true,
    State: "Restore",
    TaskBar: {
        Visible: true,
        Status: "Normal",
        Progress: 0
    },
    StartupLocation: "CenterScreen",
    Title: "WV.js Demo",
    Icon: null,
    Topmost: false,
    Muted: false,
    ClickThrough: false,
    Enabled: true,
    Visible: true,
    PreventClose: false,
    AllowDrag: true,
    AllowSnap: true,
    AllowSysMenu: true,
    HotReload: true,
    X: 100,
    Y: 100,
    Width: 800,
    Height: 600,
    StatusBar: false,
    GeneralAutofill: false,
    PasswordAutosave: false,
    PinchZoom: false,
    ZoomControl: false,
    ZoomFactor: 1,
    DefaultScriptDialogs: true,
    BrowserAcceleratorKeys: false,
    DefaultContextMenus: true
};

WV.newWindow = NewWindow;
WV_ASYNC.newWindow = NewWindowAsync;

WV.new = (name, ...args) => {
    if (name === NAME)
        return NewWindow(...args);
    
    return New(name, ...args);
}

WV_ASYNC.new = async (name, ...args) => {
    if (name === NAME)
        return await NewWindowAsync(...args);

    return await NewAsync(name, ...args);
}

function NewWindow(parameters = Default) {
    if (!isPlainObject(parameters))
        parameters = Default;

    if (parameters === WV || parameters === WV_ASYNC)
        return New(NAME);

    return New(NAME, Stringify(parameters))
}

async function NewWindowAsync(parameters = Default) {
    if (!isPlainObject(parameters))
        parameters = Default;

    if (parameters === WV || parameters === WV_ASYNC)
        return await NewAsync(NAME);

    return await NewAsync(NAME, Stringify(parameters))
}

function isPlainObject(obj) {
    if (typeof obj === 'object' && Object.prototype.toString.call(obj) === '[object Object]') {
        let props = Object.getPrototypeOf(obj);
        return props === null || props.constructor === Object;
    }
    return false;
}