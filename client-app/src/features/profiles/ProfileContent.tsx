import React from 'react'
import { Tab } from 'semantic-ui-react';
import ProfilePhotos from './ProfilePhotos';
import ProfileDesctiption from './ProfileDesctiption';
import ProfileFollowings from './ProfileFollowings';

const panes = [
  {menuItem: 'About', render: () => <ProfileDesctiption />},
  {menuItem: 'Photos', render: () => <ProfilePhotos />},
  {menuItem: 'Activities', render: () => <Tab.Pane>Activities Content</Tab.Pane>},
  {menuItem: 'Followers', render: () => <ProfileFollowings/>},
  {menuItem: 'Following', render: () => <ProfileFollowings/>}
]

const ProfileContent = () => {
  return (
    <Tab
      menu={{fluid: true, vertical: true}}
      menuPosition='right'
      panes={panes}
    />
  )
}

export default ProfileContent;