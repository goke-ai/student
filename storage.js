window.setLocalStorage = (key, value) => {

    window.localStorage.setItem(key, value);
}

window.getLocalStorage = (key, value) => {

    return window.localStorage.getItem(key);
}