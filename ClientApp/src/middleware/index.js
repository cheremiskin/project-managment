import {applyMiddleware, compose} from "redux";
import thunk from "redux-thunk";

const reduxDevtools =
    typeof window !== "undefined" && process.env.NODE_ENV !== "production"
        ? window.__REDUX_DEVTOOLS_EXTENSION__ &&
        window.__REDUX_DEVTOOLS_EXTENSION__()
        : f => f;

export const middleware = compose(
    // applyMiddleware(thunk),
    thunk
    // reduxDevtools
);
