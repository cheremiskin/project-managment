import {USER_SET_ENROLLED_PROJECTS, USER_SET_CREATED_PROJECTS, USER_SET_TOKEN, USER_SET_USER} from './actions'

const defaultState = {
    user: null,
    token: null,
    createdProjects: null,
    enrolledProjects: null,
}

export const userReducer = (state = defaultState, action) => {
    switch(action.type){
        case USER_SET_USER:
            return {...state, user: action.payload}
        case USER_SET_TOKEN:
            return {...state, token: action.payload}
        case USER_SET_CREATED_PROJECTS:
            return {...state, createdProjects: action.payload}
        case USER_SET_ENROLLED_PROJECTS:
            return {...state, enrolledProjects: action.payload}
        default:
            return state
    }
}