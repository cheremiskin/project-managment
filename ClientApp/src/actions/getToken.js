export const getToken = (token) => dispatch => {
    dispatch({type: "GET_TOKEN", payload: {token: token}})
}