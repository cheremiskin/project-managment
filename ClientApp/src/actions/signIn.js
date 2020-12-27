export const signIn = (token) => dispatch => {
    console.log('token', token);
    dispatch({type: "SIGN_IN", payload: {token: token}})
}