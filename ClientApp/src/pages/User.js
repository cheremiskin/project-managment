import React from 'react';
import HttpProvider from '../HttpProvider';

export class User extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            id: this.props.match.params.id
        }
    }
    componentDidMount() {
        // fetch(`api/users/${this.props.match.params.id}`,{
        //     method: 'GET',
        //     headers: {
        //       'Content-Type': 'application/json;charset=utf-8'
        //     }
        // }).then (
        //     (res) => {
        //         console.log(res);
        //     }
        // );

        HttpProvider.get(`/api/users/${this.props.match.params.id}`).then(
            (res) => {
                console.log(res);
            }
        )
    }

    render() {
        return (
            <div>User id: {this.props.match.params.id}</div>
        ); 
    }
};