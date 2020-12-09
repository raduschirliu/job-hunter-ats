import { CircularProgress } from '@material-ui/core';
import axios from 'axios';
import React, { useContext, useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import AuthContext from '../../contexts/AuthContext';
import { IUser } from '../../models/IUser';
import './User.css';

const User = () => {
  const { getHeaders } = useContext(AuthContext);
  const { userId } = useParams<any>();
  const [user, setUser] = useState<IUser | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    axios
      .get(`/api/users/${userId}`, getHeaders())
      .then((res) => {
        setUser(res.data);
      })
      .catch(err => console.log(err))
      .finally(() => setLoading(false));
  }, [userId, getHeaders]);

  if (loading) {
    return (
      <div className="user-container">
        <CircularProgress />
      </div>
    );
  }

  return (
    <div className="user-container">
      {user ? (
        <>
          <p>{user.userName}</p>
          <p>{user.email}</p>
          <p>{`${user.firstName} ${user.lastName}`}</p>
          <p>{user.phoneNumber}</p>
        </>
      ) : (
        <p>User does not exist :(</p>
      )}
    </div>
  );
};

export default User;
