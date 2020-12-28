export const stateToLocalStorage = store => next => action => {
    switch (action.type) {
        case 'USER_SET_TOKEN' :
            localStorage.setItem('token', JSON.stringify(action.payload));
            break
        default:
            break
    }
    
    return next(action);
}