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
          <div className="jobpost-segment">
            <p className="jobpost-title">Name</p>
            <p>{jobPost.name}</p>
          </div>
          <div className="jobpost-segment">
            <p className="jobpost-title">Description</p>
            <p>{jobPost.description}</p>
          </div>
          <div className="jobpost-segment">
            <p className="jobpost-title">Salary</p>
            <p>${jobPost.salary}</p>
          </div>
          <div className="jobpost-segment">
            <p className="jobpost-title">Closing date</p>
            <p>{jobPost.closingDate}</p>
          </div>
        </>
      ) : (
        <p>Job post does not exist :(</p>
      )}
    </div>
  );
};

export default JobPost;
