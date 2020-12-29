export const USER_SET_USER = 'USER_SET_USER'
export const USER_SET_CREATED_PROJECTS = 'USER_SET_CREATED_PROJECTS'
export const USER_SET_ENROLLED_PROJECTS = 'USER_SET_ENROLLED_PROJECTS'
export const USER_SET_TOKEN = 'USER_SET_TOKEN'
export const USER_SET_TOKEN_CHECKED = 'USER_SET_TOKEN_CHECKED'

export const setUser = (user) => {
    return {
        type: USER_SET_USER, 
        payload: user
    }
}

export const setCreatedProjects = (projects) => {
    return {
        type: USER_SET_CREATED_PROJECTS,
        payload: projects
    }
}

export const setTokenChecked = (flag) => {
    return {
        type : USER_SET_TOKEN_CHECKED,
        payload: flag
    }
}

export const setEnrolledProjects = (projects) => {
    return {
        type: USER_SET_ENROLLED_PROJECTS,
        payload: projects
    }
}

export const setToken = token => {
    return {
        type: USER_SET_TOKEN,
        payload: token
    }
}
