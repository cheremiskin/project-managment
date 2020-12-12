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
        users: (id) => `/api/projects/${id}/users`
    },
    task: {
        list: (params = {}) => addParams(`/api/tasks?`, params),
        one: (id) => `/api/tasks/${id}`,
        users: (id) => `/api/tasks/${id}/users`,
    },
    comment: {
        list: (params = {}) => addParams(`/api/comments`, params),
    }
}