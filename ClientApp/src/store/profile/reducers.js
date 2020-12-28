import {PROFILE_SET_CREATED_PROJECTS, PROFILE_SET_ENROLLED_PROJECTS, PROFILE_SET_USER} from "./actions";

export {PROFILE_SET_ENROLLED_PROJECTS, PROFILE_SET_CREATED_PROJECTS, PROFILE_SET_USER} from './actions'

const defaultState = {
    user: null,
    createdProjects : null ,
    enrolledProjects : null 
}

export const profileReducer = (state = defaultState, action) => {
    switch(action.type) {
        case  PROFILE_SET_USER:
            return {...state, user: action.payload}
        case PROFILE_SET_CREATED_PROJECTS:
            return {...state, createdProjects: action.payload}
        case PROFILE_SET_ENROLLED_PROJECTS:
            return {...state, enrolledProjects: action.payload}
        default:
            return state;
    }
}