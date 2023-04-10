const NAME = 'Ports';
const WV = window.wv;
const WV_ASYNC = WV.async();
const New = WV.new;
const NewAsync = WV_ASYNC.new;
let PortsObj = null;
let PortsAsync = null;

Object.defineProperty(WV, NAME, {
    get() {
        TryCreate();
        return PortsObj;
    }
});

Object.defineProperty(WV_ASYNC, NAME, {
    get() {
        TryCreate();
        return PortsAsync;
    }
});

WV.new = (name, ...args) => {
    if (name === NAME)
        return WV[NAME];

    return New(name, ...args);
}

WV_ASYNC.new = async (name, ...args) => {
    if (name === NAME)
        return WV_ASYNC[NAME];

    return await NewAsync(name, ...args);
}

function TryCreate() {
    if (PortsObj !== null)
        return;

    if (!WV.IsPluginsEnabled)
        return;

    try {
        PortsObj = New(NAME);
        PortsAsync = PortsObj.async();
    } catch (e) { }
}