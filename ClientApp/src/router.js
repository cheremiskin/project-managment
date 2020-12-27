const addParams = (url, params = {}) => {
    if (params) {
        let query = Object.keys(params)
            .map(param => encodeURIComponent(param) + '=' + encodeURIComponent(params[param]))
            .join('&');
        url += query;
    }
    return url;
}

export const router = {
    project: {
        list: (params = {}) => addParams(`/api/projects`, params),
        one: (id) => `api/projects/${id}`,
        users: (id) => `/api/projects/${id}/users`,
        addUser: (projectId, userId) => addParams(`/api/projects/${projectId}/users?`, {userId: userId})
    },
    task: {
        list: (params = {}) => addParams(`/api/tasks?`, params),
        one: (id) => `/api/tasks/${id}`,
        users: (id) => `/api/tasks/${id}/users`,
        addUser: (taskId, userId) => addParams(`/api/tasks/${taskId}/users?`, {userId}),
        create: (params = {}) => addParams('/api/tasks?', params),
        statuses: () => `/api/tasks/statuses`
    },
    comment: {
        list: (params = {}) => addParams(`/api/comments?`, params),
        one: (id) => `/api/comments/${id}`
    },
    user: {
        me : () => `/api/users/me`,
        one: (id) => `/api/users/${id}`,
        many: (page, size) => addParams(`/api/users?`, {page : page, size: size}),
        createdProjects: (id) => `/api/users/${id}/created-projects`,
        enrolledProjects: (id) => `/api/users/${id}/enrolled-projects`,
        myProjects: () => `/api/users/my-projects`
    }
}