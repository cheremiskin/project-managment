export const getUser = (user = {}) => dispatch => {
    dispatch({type: "GET_USER", payload: {user: user}})
}