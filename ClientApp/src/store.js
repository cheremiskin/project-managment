import { applyMiddleware, createStore } from "redux";
import reducers from "./reducers";
// import { middleware } from "./middleware";
import thunk from "redux-thunk";

export const makeStore = initialState => {
    return createStore(reducers, {}, applyMiddleware(thunk));
};