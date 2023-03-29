const WIN = window;
const NAME = '-Name-';
const WV = WIN[NAME].WV;
const HO = WIN[NAME].HO;
const SYNC = WIN[NAME].SYNC;
const ASYNC = WIN[NAME].ASYNC;
const ISFRAME = WIN[NAME].ISFRAME;
const UID = SYNC.UID;
const TO_STRING = Object.prototype.toString;
const STRINGIFY = JSON.stringify;
const ARRAY_FROM = Array.from;
const IS_ARRAY = Array.isArray;
const ARRAYS = ['Uint8Array', 'Uint8ClampedArray', 'Int8Array', 'Uint16Array', 'Int16Array', 'Uint32Array', 'Int32Array', 'Float32Array', 'Float64Array', 'BigUint64Array', 'BigInt64Array'];

WIN[NAME] = {
    SendMessage,
    New,
    Kill,
    NewAsync,
    KillAsync,
    ToJSValue,
    ToJSValueAsync,
    Recover,
    RecoverAsync,
    get UID() {
        return UID;
    },
    get IsFrame() {
        return ISFRAME;
    },
    get EnablePlugins() {
        return SYNC.EnablePlugins;
    },
    get Screens() {
        return SYNC.Screens;
    }
};

//#region PUBLIC FUNCTIONS

function SendMessage(message) {
    try {
        WV.postMessage(message);
    } catch (e) {
        WV.postMessage(message + '');
    }
}

function New(name, ...args){
    args.unshift(name)
    const result = SYNC.EC.apply(SYNC, args);
    return ThereIsError(result);
}

function Kill(uid){
    const result = SYNC.EK(uid);
    return ThereIsError(result);
}

async function NewAsync(name, ...args){
    args.unshift(name)
    const result = await ASYNC.EC.apply(ASYNC, args);
    return ThereIsError(result);
}

async function KillAsync(uid){
    const result = await ASYNC.EK(uid);
    return ThereIsError(result);
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

function Recover(uid) {
    const result = SYNC.ER(uid + '');
    return ThereIsError(result);
}

async function RecoverAsync(uid) {
    const result = await ASYNC.ER(uid + '');
    return ThereIsError(result);
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
    if (typeof obj === 'object' && ToString(obj) === '[object Object]') {
        let props = Object.getPrototypeOf(obj);
        return props === null || props.constructor === Object;
    }
    return false;
}

/**
 * https://gist.github.com/Raiondesu/d4491ae05b46ea32d6d803066f9d7997
 * Credit to https://stackoverflow.com/a/49651719 and https://stackoverflow.com/a/60323358
 * @param {object} obj 
 * @returns 
 */
function IsProxy(obj){
    try {
        postMessage(obj, window);
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
    const YY = date.getFullYear();
    const MM = ToString(date.getMonth() + 1).padStart(2, 0);
    const DD = ToString(date.getDate()).padStart(2, 0);
    const hh = ToString(date.getHours()).padStart(2, 0);
    const mm = ToString(date.getMinutes()).padStart(2, 0);
    const ss = ToString(date.getSeconds()).padStart(2, 0);

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

    if (IsProxy(obj))
        return 'Proxy';

    if (IsPlainObject(obj))
        return 'Object';

    if (typeof obj === 'symbol')
        return 'Symbol';

    return obj.constructor.name;
}

//#endregion
