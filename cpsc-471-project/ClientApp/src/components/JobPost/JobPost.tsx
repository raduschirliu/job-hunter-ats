import { CircularProgress } from '@material-ui/core';
import axios from 'axios';
import React, { useContext, useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import AuthContext from '../../contexts/AuthContext';
import { IJobPost } from '../../models/IJobPost';
import './JobPost.css';

const JobPost = () => {
  const { getHeaders } = useContext(AuthContext);
  const { jobPostId } = useParams<any>();
  const [jobPost, setJobPost] = useState<IJobPost | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    axios
      .get(`/api/jobposts/${jobPostId}`, getHeaders())
      .then((res) => {
        setJobPost(res.data);
      })
      .catch((err) => console.log(err))
      .finally(() => setLoading(false));
  }, [jobPostId, getHeaders]);

  if (loading) {
    return (
      <div className="jobpost-container">
        <CircularProgress />
      </div>
    );
  }

  return (
    <div className="jobpost-container">
      {jobPost ? (
        <>
          <p>{jobPost.name}</p>
          <p>{jobPost.description}</p>
          <p>{jobPost.salary}</p>
          <p>{jobPost.closingDate}</p>
        </>
      ) : (
        <p>Job post does not exist :(</p>
      )}
    </div>
  );
};

export default JobPost;
