import { List, Avatar, Button, Skeleton, Typography } from 'antd';

import React, {useEffect, useState} from "react";
import HttpProvider from "../HttpProvider";
import {router} from "../router";
import {Link} from "react-router-dom";
import {connect} from 'react-redux'

const {Paragraph} = Typography

const pageSize = 3

export const UserList = (props) => {
    const {token} = props
    
    const [initLoading, setInitLoading] = useState(true)
    const [loading, setLoading] = useState(false)
    const [data, setData] = useState([])
    const [page, setPage] = useState(0)
    
    const loadData = callback => {
        HttpProvider.get(router.user.many(page, pageSize)).then(
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

        <List
            loading={initLoading}
            itemLayout="horizontal"
            loadMore={loadMore}
            dataSource={data}
            renderItem= {user => (
                <List.Item
                    actions={[<Link tag = {Link} to = {`/user/${user.id}`}>profile</Link>]}
                >
                    <Skeleton avatar title={'...'} loading = {false} active>
                        <List.Item.Meta
                            avatar={ <Avatar src="https://picsum.photos/200/200" /> }
                            title={<a href={`/user/${user.id}`}>{user.fullName}</a>}
                            description={<Paragraph ellipsis = {{
                                rows: 1,
                                expandable: true,
                                symbol: 'more'
                            }}>{user.info}</Paragraph>}
                        />
                    </Skeleton>
                </List.Item>
            )}
        />
    )
}

const mapStateToProps = (state) => {
    return {
        token: state.user.token
    }
}

export default connect(mapStateToProps, {})(UserList)
