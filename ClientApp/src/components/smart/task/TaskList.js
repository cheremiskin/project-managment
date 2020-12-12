import React, { useEffect, useState } from 'react';
import HttpProvider from '../../../HttpProvider';
import { router } from '../../../router';
import TaskCard from '../../dumb/task/TaskCard';

const TaskList = (props) => {

    const [tasks, setTasks] = useState([]);

    useEffect(()=>{
        HttpProvider.get(router.task.list(props.params)).then(res => {
            // setTasks(res.map((item, index) => <TaskCard {...item} key={index} />))
        })
    }, []);

    return (
        <>
            <h3>Tasks</h3>
            {tasks}
        </>
    )
}

export default TaskList;