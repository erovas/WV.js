const WIN = window;
const NAME = '-Name-';
const WV = WIN[NAME].WV;
const HO = WIN[NAME].HO;
const SYNC = WIN[NAME].SYNC;
const ASYNC = WIN[NAME].ASYNC;
const ISFRAME = WIN[NAME].ISFRAME;
const UID = SYNC.UID;
const PLATFORM = SYNC.Platform;
const VERSION = SYNC.Version;
const ISMAINWEBVIEW = SYNC.IsMainWebView;
const ARGS = SYNC.Args;
const TO_STRING = Object.toString;
const STRINGIFY = JSON.stringify;
const ARRAY_FROM = Array.from;
const IS_ARRAY = Array.isArray;
const ARRAYS = ['Uint8Array', 'Uint8ClampedArray', 'Int8Array', 'Uint16Array', 'Int16Array', 'Uint32Array', 'Int32Array', 'Float32Array', 'Float64Array', 'BigUint64Array', 'BigInt64Array'];

const EXPOSE_ASYNC = Object.create(null);
const EXPOSE = Object.create(null);

Object.defineProperties(EXPOSE, {
    Shutdown: {
        value: () => SYNC.Shutdown()
    },
    Restart: {
        value: () => SYNC.Restart()
    },
    SendMessage: {
        value: SendMessage
    },
    ToJSValue: {
        value: ToJSValue
    },
    Platform: {
        get: () => PLATFORM
    },
    Version: {
        get: () => VERSION
    },
    Args: {
        get: () => [...ARGS]
    },
    CurrentScreen: {
        get: () => SYNC.CurrentScreen
    },
    Screens: {
        get: () => SYNC.Screens
    },
    IsMainWebView: {
        get: () => ISMAINWEBVIEW
    },
    IsFrame: {
        get: () => ISFRAME
    },
    IsPluginsEnabled: {
        get: () => SYNC.IsPluginsEnabled
    },
    new: {
        value: (name, ...args) => {
            args.unshift(name)
            const result = SYNC.EC.apply(SYNC, args);
            return ThereIsError(result);
        }
    },
    async: {
        value: () => EXPOSE_ASYNC
    }
});

Object.defineProperties(EXPOSE_ASYNC, {
    Shutdown: {
        value: async () => await ASYNC.Shutdown()
    },
    Restart: {
        value: async () => await ASYNC.Restart()
    },
    SendMessage: {
        value: async message => SendMessage(message)
    },
    ToJSValue: {
        value: ToJSValueAsync
    },
    Platform: {
        get: async () => PLATFORM
    },
    Version: {
        get: async () => VERSION
    },
    Args: {
        get: async () => [...ARGS]
    },
    CurrentScreen: {
        get: async () => await ASYNC.CurrentScreen
    },
    Screens: {
        get: async () => await ASYNC.Screens
    },
    IsMainWebView: {
        get: async () => ISMAINWEBVIEW
    },
    IsFrame: {
        get: async () => ISFRAME
    },
    IsPluginsEnabled: {
        get: async () => await ASYNC.IsPluginsEnabled
    },
    new: {
        value: async (name, ...args) => {
            args.unshift(name)
            const result = await ASYNC.EC.apply(SYNC, args);
            return ThereIsError(result);
        }
    },
    sync: {
        value: async () => EXPOSE
    }
})

if (ISFRAME) {
    delete EXPOSE.Shutdown;
    delete EXPOSE.Restart;
    delete EXPOSE.IsMainWebView;
    delete EXPOSE.Args;

    delete EXPOSE_ASYNC.Shutdown;
    delete EXPOSE_ASYNC.Restart;
    delete EXPOSE_ASYNC.IsMainWebView;
    delete EXPOSE_ASYNC.Args;
}

Object.defineProperty(WIN, NAME, {
    get() {
        return EXPOSE;
    }
});

//#region PUBLIC FUNCTIONS

function SendMessage(message) {
    try {
        return WV.postMessage(message);
    } catch (e) {
        return WV.postMessage(message + '');
    }
}

function ToJSValue(obj){
    const jsType = GetJSType(obj);
    const raw = obj;
    let csParsed = null;
    let stringified = null;

    if (jsType === Object.name){
        const args = [raw, STRINGIFY(raw)];
        const names = Object.getOwnPropertyNames(obj);

        for (let i = 0; i < names.length; i++) {
            const name = names[i];
            const value = MustTF(raw[name]);
            args.push(name, value);
        }

        const output = SYNC.TFO.apply(SYNC, args);
        return ThereIsError(output);
    }
        
    
    switch (jsType) {

        case Array.name:
            csParsed = MustTF(raw);
            stringified = STRINGIFY(raw);
            break;

        case (null + ''):
        case (undefined + ''):
            stringified = raw + '';
            break;

        case Symbol.name:
        case Function.name:
            stringified = ToString(raw);
            break;

        case BigInt.name:
            csParsed = ToString(raw);
            break;

        case Date.name:
            csParsed = DateToString(raw);
            stringified = STRINGIFY(raw);
            break;
    
        default:

            for (let i = 0; i < ARRAYS.length - 2; i++)
                if(jsType === ARRAYS[i] && i < ARRAYS.length - 2){
                    csParsed = ARRAY_FROM(raw);
                    break;
                }
            
            // 'BigUint64Array', 'BigInt64Array'
            if (!IS_ARRAY(csParsed)) 
                for (let i = ARRAYS.length - 2; i < ARRAYS.length; i++)
                    if(jsType === ARRAYS[i]){
                        csParsed = [];
                        for (let j = 0; j < raw.length; j++) 
                            csParsed.push(ToString(raw[j]));
                        break;
                    }

            // Custom
            if (!IS_ARRAY(csParsed))
                // Tiene definido como jsonearlo
                if(raw.toJSON !== undefined)
                    stringified = STRINGIFY(raw);
        
                // Por si han machacado el metodo original
                else if(typeof raw.toString !== 'function')
                    return ToString(raw);

                // Por defecto
                else
                    stringified = raw.toString();
            

            break;
    }
    
    const output = SYNC.TF(jsType, raw, csParsed, stringified);
    return ThereIsError(output);
}

async function ToJSValueAsync(obj){
    const jsType = GetJSType(obj);
    const raw = obj;
    let csParsed = null;
    let stringified = null;

    if (jsType === Object.name){
        const args = [raw, STRINGIFY(raw)];
        const names = Object.getOwnPropertyNames(obj);

        for (let i = 0; i < names.length; i++) {
            const name = names[i];
            const value = await MustTFAsync(raw[name]);
            args.push(name, value);
        }

        const output = await ASYNC.TFO.apply(ASYNC, args);
        return ThereIsError(output);
    }
    
    switch (jsType) {

        case Array.name:
            csParsed = await MustTFAsync(raw);
            stringified = STRINGIFY(raw);
            break;

        case (null + ''):
        case (undefined + ''):
            stringified = raw + '';
            break;

        case Symbol.name:
        case Function.name:
            stringified = ToString(raw);
            break;

        case BigInt.name:
            csParsed = ToString(raw);
            break;

        case Date.name:
            csParsed = DateToString(raw);
            stringified = STRINGIFY(raw);
            break;
    
        default:

            for (let i = 0; i < ARRAYS.length - 2; i++)
                if(jstype === ARRAYS[i] && i < ARRAYS.length - 2){
                    csParsed = ARRAY_FROM(raw);
                    break;
                }
            
            // 'BigUint64Array', 'BigInt64Array'
            if (!IS_ARRAY(csParsed)) 
                for (let i = ARRAYS.length - 2; i < ARRAYS.length; i++)
                    if(jstype === ARRAYS[i]){
                        csParsed = [];
                        for (let j = 0; j < raw.length; j++) 
                            csParsed.push(ToString(raw[j]));
                        break;
                    }

            // Custom
            if (!IS_ARRAY(csParsed))
                // Tiene definido como jsonearlo
                if(raw.toJSON !== undefined)
                    stringified = STRINGIFY(raw);
        
                // Por si han machacado el metodo original
                else if(typeof raw.toString !== 'function')
                    return ToString(raw);

                // Por defecto
                else
                    stringified = raw.toString();
            

            break;
    }
    
    const output = await ASYNC.TF(jsType, raw, csParsed, stringified);
    return ThereIsError(output);
}

//#endregion

//#region PRIVATE FUNCTIONS

function ThereIsError(obj){
    if(typeof obj === 'string')
        throw new Error(obj);

    return obj;
}

function ToString(obj, ...args){
    return TO_STRING.apply(obj, args);
}

function IsPlainObject(obj){
    if (typeof obj === 'object' && Object.prototype.toString.call(obj) === '[object Object]') {
        let props = Object.getPrototypeOf(obj);
        return props === null || props.constructor === Object;
    }
    return false;
}

/**
 * Modified from https://gist.github.com/Raiondesu/d4491ae05b46ea32d6d803066f9d7997
 * Credit to https://stackoverflow.com/a/49651719 and https://stackoverflow.com/a/60323358
 * @param {object} obj 
 * @returns 
 */
function IsProxy(obj) {
    if (typeof obj === 'function')
        return false;

    try {
        postMessage(obj, WIN);
    } catch (error) {
        return error && error.code === 25;
    }
    
    return false;
}

/**
 * 
 * @param {Date} date 
 */
function DateToString(date){
    const YY = date.getUTCFullYear();
    const MM = ToString(date.getUTCMonth() + 1).padStart(2, 0);
    const DD = ToString(date.getUTCDate()).padStart(2, 0);
    const hh = ToString(date.getUTCHours()).padStart(2, 0);
    const mm = ToString(date.getUTCMinutes()).padStart(2, 0);
    const ss = ToString(date.getUTCSeconds()).padStart(2, 0);

    return DD + '/' + MM + '/' + YY + ' ' + hh + ':' + mm + ':' + ss;
}

function MustTF(obj){

    switch (GetJSType(obj)) {
        case (null + ''):
        case Number.name:
        case String.name:
        case Date.name:
            return obj;

        case (undefined + ''):
        case Symbol.name:
        case Function.name:
        case BigInt.name:
        case Object.name:
            return ToJSValue(obj);
    
        case Array.name:
            const output = [];
            for (let i = 0; i < obj.length; i++) {
                const element = obj[i];
                output.push(MustTF(element));
            }
            return output;

        default:
            return ToJSValue(obj);
    }
}

async function MustTFAsync(obj){

    switch (GetJSType(obj)) {
        case (null + ''):
        case Number.name:
        case String.name:
        case Date.name:
            return obj;

        case (undefined + ''):
        case Symbol.name:
        case Function.name:
        case BigInt.name:
        case Object.name:
            return await ToJSValueAsync(obj);
    
        case Array.name:
            const output = [];
            for (let i = 0; i < obj.length; i++) {
                const element = obj[i];
                output.push(await MustTFAsync(element));
            }
            return output;

        default:
            return await ToJSValueAsync(obj);
    }
}

function GetJSType(obj){
    if (obj === null || obj === undefined)
        return obj + '';

    if (IsPlainObject(obj))
        return 'Object';

    if (typeof obj === 'symbol')
        return 'Symbol';

    if (IsProxy(obj))
        return 'Proxy';

    return obj.constructor.name;
}

//#endregion
