const NAME = 'Storage';
const WV = window.wv;
const WV_ASYNC = WV.async();
const New = WV.new;
const NewAsync = WV_ASYNC.new;
let StorageObj = null;
let StorageAsync = null;

Object.defineProperty(WV, NAME, {
    get() {
        TryCreate();
        return StorageObj;
    }
});

Object.defineProperty(WV_ASYNC, NAME, {
    get() {
        TryCreate();
        return StorageAsync;
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
    if (StorageObj !== null)
        return;

    if (!WV.IsPluginsEnabled)
        return;

    try {
        StorageObj = New(NAME);
        StorageAsync = StorageObj.async();
    } catch (e) { }
}