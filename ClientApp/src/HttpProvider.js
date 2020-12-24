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
        console.log('POST: ', data);
        console.log('Headers', headers);
        return fetch(`${API_ROOT}${url}`, {
            method: method,
            // mode: 'cors',
            // headers : new Headers(...headers),
            headers : {
                'Content-Type': 'application/json;charset=UTF-8',
                ...headers
            },
            body : JSON.stringify(data)
        }).then((response) => {

            if (!response.ok) {
                const error = new Error(response.statusText);
                console.error("error: ", error);
            }

            console.log('response: ', response)
            return response;
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
        console.log('POST WITH LINK', url)
        return this._send(url, 'POST', data, {'Authorization': token});
    }

    static auth_put(url, data, token = ''){
        return this._send(url, 'PUT', data, {'Authorization': token})
    }
    
    static auth_delete(url, token = ''){
        return this._send(url, 'DELETE', {}, {'Authorization': token})
    }

}

export default HttpProvider;