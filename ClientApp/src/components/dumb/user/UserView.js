import React from 'react'
import {Avatar} from 'antd'

import '../../../assets/styles/components/UserView.css'

export const UserView = (props) => {
    const [firstName, lastName] = props.user.fullName.split(' ')
    const info = props.user.info

    return (
        <div className = 'user-wrapper'>
            <Avatar style = {{backgroundColor: props.avatarColor}}> {firstName[0]}</Avatar>
            {props.withFullName  &&
            <div className = 'username'>
                <div> {firstName} </div>
                <div> {lastName}</div>
            </div>
            } 
            {props.withInfo &&
                <div className = 'info'>
                    {info}
                </div>
            }
        </div>
    ) 
}

