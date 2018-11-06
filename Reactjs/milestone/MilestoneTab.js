import React from 'react'
import * as milestoneService from '../../../services/milestoneService'
import * as userService from '../../../services/userService'
import MentorAddMilestoneModal from './MentorMilestoneForm';
import swal from 'sweetalert2'
import { connect } from 'react-redux'
import { userTypes } from '../../../constants/userTypes';
import moment from 'moment';

class MilestoneTab extends React.Component {
    constructor(props) {
        super(props)

        this.state = {
            mentorId: '',
            userId: '',
            milestoneList: [],
            currentUserId: '',
            typeId: ''
        }
        this.handleChecked = this.handleChecked.bind(this);
        this.getMilestoneByUserId = this.getMilestoneByUserId.bind(this);
        this.deleteMilestone = this.deleteMilestone.bind(this);
        this.usersMentorsMatchCheck = this.usersMentorsMatchCheck.bind(this);
    }

    componentDidMount() {
        let typeId = this.props.currentUserProfile.userTypeId
        this.setState({
            currentUserId: this.props.currentUserProfile.userId,
            userId: this.props.userId,
            mentorId: this.props.mentorId,
            typeId: typeId
        })

        this.getMilestoneByUserId(typeId);
        this.usersMentorsMatchCheck();
    }

    handleChecked(e) {
        let data = {
            isCompleted: e.target.checked
        };
        milestoneService.updateUserMilestoneIsCompleted(e.target.id, data)
            .then(() => this.getMilestoneByUserId(this.state.typeId))
            .catch(console.error);
    }

    getMilestoneByUserId(param) {
        if (parseInt(param) === userTypes.mentor) {
            milestoneService.userMilestoneGetByUserIdAndMentorId(this.props.userId, this.props.mentorId)
                .then(response => {
                    this.setState({
                        milestoneList: response.items
                    })
                })
        } else {
            milestoneService.userMilestoneGetByUserId(this.props.userId)
                .then(response => {
                    this.setState({
                        milestoneList: response.items
                    })
                })
                .catch(console.error);
        }
    }

    usersMentorsMatchCheck() {
        if (this.props.isMentor) {
            userService.usersMentors_GetByMentorId(this.props.mentorId)
                .then(response => {
                    this.setState({
                        mentorMatchCheck: response.items.includes(this.props.userId)
                    })
                })
                .catch(console.error);
        }
    }

    deleteMilestone(id) {
        swal({
            title: "Are you sure you want to delete this milestone?",
            text: "You won't be able to recover this!",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "red",
            cancelButtonColor: '#7ac7f6',
            confirmButtonText: "Yes, delete it!",
            cancelButtonText: "No, keep it!",
            background: '#0f2940'
          }).then((result) => {
            if (result.value) {
                milestoneService.deleteUserMilestoneById(id)
                    .then(() => this.getMilestoneByUserId(this.state.typeId));
                    swal({
                        title: "Deleted!",
                        text: "Milestone has been deleted.",
                        type: "success",
                        background: '#0f2940',
                        confirmButtonColor: '#7ac7f6'
                      })
            } 
        })
    }

    render() {
        const milestones = this.state.milestoneList.map((milestone, index) => {
            return (
                <tr key={milestone.id} >
                    <td>{index + 1}</td>
                    <td>{milestone.lastName},{milestone.firstName}</td>
                    <td>{milestone.description}</td>
                    <td>{milestone.isCompleted ? "yes" : "no"}</td>
                    <td>
                        {(!this.props.isMentor && this.props.userId === this.state.currentUserId) || this.props.isAdmin ?
                            <label className="custom-control custom-checkbox">
                                <input type="checkbox"
                                    className="custom-control-input"
                                    id={milestone.id}
                                    value={milestone.isCompleted}
                                    defaultChecked={milestone.isCompleted}
                                    onChange={this.handleChecked} />
                                <span className="custom-control-indicator"></span>
                            </label>
                            : null}
                    </td>
                    <td>{milestone.isCompleted ? moment(milestone.dateModified).utc().format('MM-DD-YYYY'):""}</td>
                    <td>
                        {this.props.mentorId === milestone.mentorId || this.props.isAdmin ? <button id={milestone.id}
                            className="btn btn-light btn--icon" onClick={(e) => this.deleteMilestone(milestone.id)}>
                            <i className="zmdi zmdi-delete zmdi-hc-fw"></i></button> : null}
                    </td>
                </tr>
            )
        })

        return (
            <span>
                {(this.props.isMentor && this.state.mentorMatchCheck) || this.props.isAdmin
                    ? <MentorAddMilestoneModal getMilestoneByUserId={e => { this.getMilestoneByUserId(this.state.typeId) }}
                        userId={this.props.userId}
                        mentorId={this.props.mentorId} /> : null}
                <table
                    className="table"
                >
                    <thead>
                        <tr>
                            <th>Milestone#</th><th>Issued By</th>
                            <th>Description</th><th>Is Completed</th>
                            <th></th><th>Date Completed</th>
                        </tr>
                    </thead>

                    <tbody>
                        {milestones}
                    </tbody>
                </table>
            </span>
        )
    }
}

const mapStateToProps = state => ({
    currentUserProfile: state.userProfiles
})

export default connect(mapStateToProps)(MilestoneTab);
