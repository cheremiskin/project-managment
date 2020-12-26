import React from 'react';
import {Comment, Avatar} from 'antd'
import {Link} from "react-router-dom";
import moment from "moment";

// time, content, user

export const TaskComment = (props) => {
    
    const {comment, user} = props 
    
    
    return (
       <Comment
           author = {<Link tag = {Link} to={`/user/${user.id}`}> {user.fullName}</Link>}
           avatar = {<Avatar src = 'https://picsum.photos/200'/>}
           content = {<p>{comment.content}</p>} 
           datetime = {moment(comment.creationDate).format('YYYY-MM-DD HH:mm')}
       />
           
    )
    
}