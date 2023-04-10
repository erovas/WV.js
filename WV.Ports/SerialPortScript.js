const NAME = 'SerialPort';
const WV = window.wv;
const WV_ASYNC = WV.async();
const New = WV.new;
const NewAsync = WV_ASYNC.new;

WV.newSerialPort = NewSerialPort;
WV_ASYNC.newSerialPort = NewSerialPortAsync;

WV.new = (name, ...args) => {
    if (name === NAME)
        return NewSerialPort(...args);

    return New(name, ...args);
}

WV_ASYNC.new = async (name, ...args) => {
    if (name === NAME)
        return await NewSerialPortAsync(...args);

    return await NewAsync(name, ...args);
}

function NewSerialPort(portName = '', baudRate = 0, parity = '', dataBits = 0) {
    return New(NAME, ...arguments);
}

async function NewSerialPortAsync(portName = '', baudRate = 0, parity = '', dataBits = 0) {
    return await NewAsync(NAME, ...arguments);
}