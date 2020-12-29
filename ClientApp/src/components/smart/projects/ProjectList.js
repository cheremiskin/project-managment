import { List, Avatar, Button, Skeleton, Typography } from 'antd';
import React, {useEffect, useState} from "react";
import HttpProvider from "../../../HttpProvider";
import {router} from "../../../router";
import {Link} from "react-router-dom";
import {connect} from 'react-redux'
import ProjectCard from '../../dumb/project/ProjectCard';

const {Paragraph} = Typography

const pageSize = 3

export const ProjectList = (props) => {
    const {token} = props
    
    const [initLoading, setInitLoading] = useState(true)
    const [loading, setLoading] = useState(false)
    const [data, setData] = useState([])
    const [page, setPage] = useState(0)
    
    const loadData = callback => {
        HttpProvider.get(router.project.many(page, pageSize)).then(
            result => callback(result)
        )
    }
    
    useEffect(() => {   
        loadData(result => {
            if (result.length > 0){
                setPage(prevPage => prevPage + 1)
                setData(result)
                setInitLoading(false)
            }
        })   
    }, [])
    
    const onLoadMore = () => {
        setLoading(true)
        loadData((data) => {
            setData(prevData => prevData.concat(data))
            setPage(prevPage => prevPage + 1)
            setLoading(false)
        })
    }
    
    const loadMore = !initLoading && !loading ? (
        <div
            style={{
                textAlign: 'center',
                marginTop: 12,
                height: 32,
                lineHeight: '32px',
            }} 
        >
            <Button onClick={onLoadMore}>load more</Button>
        </div>
        ) : null;

    return (
        <>
        {
            props.withLoadMore && 
            <List
                loading={initLoading}
                itemLayout="horizontal"
                loadMore={loadMore}
                dataSource={data}
                renderItem= {project => (
                    <List.Item>
                        <Skeleton loading = {false}>
                            <ProjectCard {...project} />
                        </Skeleton>
                    </List.Item>
                )}
            />
        }
        {
            !props.withLoadMore &&
            props.projects.map((item, index) => <ProjectCard {...item} key={index} />)
        }
        </>
    )
}

const mapStateToProps = (state) => {
    return {
        token: state.user.token
    }
}

export default connect(mapStateToProps)(ProjectList)
