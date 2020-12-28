export const PROFILE_SET_USER = 'USER_SET_USER'
export const PROFILE_SET_CREATED_PROJECTS = 'PROFILE_SET_CREATED_PROJECTS'
export const PROFILE_SET_ENROLLED_PROJECTS = 'PROFILE_SET_ENROLLED_PROJECTS'

export const setProfileUser = (user) => {
    return {
        type : PROFILE_SET_USER,
        payload: user 
    }
}

export const setProfileCreatedProjects = (projects) => {
    return{
        type: PROFILE_SET_CREATED_PROJECTS,
        payload: projects
    }
}

export const setProfileEnrolledProjects = (projects) => {
    return{
        type: PROFILE_SET_ENROLLED_PROJECTS,
        payload: projects
    }
}
