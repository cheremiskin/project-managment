export const USER_SET_USER = 'USER_SET_USER'
export const USER_SET_CREATED_PROJECTS = 'USER_SET_CREATED_PROJECTS'
export const USER_SET_ENROLLED_PROJECTS = 'USER_SET_ENROLLED_PROJECTS'
export const USER_SET_TOKEN = 'USER_SET_TOKEN'

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
