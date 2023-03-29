const Name = 'Window';
const New = wv.New;
const NewAsync = wv.NewAsync;

wv.New = function (name, ...args) {
    if (name === Name && args.length > 0) {
        try {
            args = JSON.stringify(args[0]);
        } catch (e) {
            args = args + '';
        }

        return New(name, args);
    }

    return New(name, ...args);
}

wv.NewAsync = async function (name, ...args) {
    if (name === Name && args.length > 0) {
        try {
            args = JSON.stringify(args[0]);
        } catch (e) {
            args = args + '';
        }

        return await NewAsync(name, args);
    }

    return await NewAsync(name, ...args);
}