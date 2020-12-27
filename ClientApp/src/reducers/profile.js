const initialState = {
    user: null,
    token: null,
    count: 0
}

export default (state = initialState, action) => {
    switch (action.type) {
        case 'SIGN_IN':
            return {
                ...state,
                ...action.payload
            }
        case 'COUNT':
            return {
                ...state,
                count: state.count + 1
            }
        default:
            return state
    }
}