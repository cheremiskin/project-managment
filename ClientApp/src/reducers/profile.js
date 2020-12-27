const initialState = {
    user: null,
    token: null,
    count: 0
}

export default (state = initialState, action) => {
    switch (action.type) {
        case 'GET_TOKEN':
            return {
                ...state,
                ...action.payload
            }
        case 'GET_USER':
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