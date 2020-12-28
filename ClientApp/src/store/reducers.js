import {combineReducers} from 'redux';
import {userReducer} from "./user/reducers";
import {profileReducer} from "./profile/reducers";

export default combineReducers({
    user: userReducer,
    profile: profileReducer
})