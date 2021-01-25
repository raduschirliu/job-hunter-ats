import { CircularProgress } from '@material-ui/core';
import axios from 'axios';
import React, { useContext, useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import AuthContext from '../../contexts/AuthContext';
import IUser from '../../models/IUser';

import './Users.css';

/**
 * UserCard component, responsible to display a single user card
 * @param user User object
 */
const UserCard = ({ user }: { user: IUser }) => {
  return (
    <Link to={`/users/${user.userName}`}>
      <div className="user-card">
        <p>{`${user.firstName} ${user.lastName}`}</p>
      </div>
    </Link>
  );
};

/**
 * Users component, responsible to fetch and display the list of all registered users
 */
const Users = () => {
  const { getHeaders } = useContext(AuthContext);
  const [users, setUsers] = useState<IUser[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (loading) setLoading(true);
    axios
      .get('/api/users', getHeaders())
      .then((res) => {
        console.log(res.data);
        setUsers(res.data);
      })
      .catch((err) => console.log(err))
      .finally(() => setLoading(false));
  }, [loading, getHeaders]);

  return (
    <div className="users-container">
      {loading ? (
        <CircularProgress />
      ) : (
        <>
          {users.map((user) => (
            <UserCard key={user.userName} user={user} />
          ))}
        </>
      )}
    </div>
  );
};

export default Users;
