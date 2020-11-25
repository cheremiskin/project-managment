// const fetch = require('../node-fetch');
const API_ROOT = '';

class HttpProvider {

    static get(url, params = {}) {
        return fetch(`${API_ROOT}${url}`, params).then((response) => {

            if (!response.ok) {
                const error = new Error(response.statusText);
                console.error(error);
            }

            return response.json();
        })
    }

    static _send(url, method = 'POST', data = {}, headers = {}) {
        console.log(data);
        return fetch(`${API_ROOT}${url}`, {
            method,
            headers : {
                'Content-Type': 'application/json;charset=UTF-8',
                ...headers
            },
            body : JSON.stringify(data)
        }).then((response) => {

            if (!response.ok) {
                const error = new Error(response.statusText);
                console.error(error);
            }

            return response.json();
        })
    }

    static post(url, data = {}) {
        return this._send(url, 'POST', data);
	}

	static put(url, data = {}) {
		return this._send(url, 'PUT', data);
	}

	static del(url) {
		return this._send(url, 'DELETE');
    }
    
    static auth(url, token = '') {
        return this.get(url, {headers: {'Authorization': token}});
    }

    static auth_post(url, data, token = '') {
        return this._send(url, 'POST', data, {'Authorization': token});
    }

}

export default HttpProvider;