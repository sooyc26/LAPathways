import React from "react";
import { Modal, Button, Checkbox, HelpBlock, FormControl, FormGroup } from 'react-bootstrap'
import * as milestoneService from '../../../services/milestoneService'

export default class MentorAddMilestoneModal extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            show: false,
            showInput: false,
            milestones: [],
            description: '',
            checkedList: [],

            validate: ''
        }
        this.handleShow = this.handleShow.bind(this);
        this.handleClose = this.handleClose.bind(this);
        this.handleInputOpen = this.handleInputOpen.bind(this);
        this.handleInputClose = this.handleInputClose.bind(this);

        this.handleChecked = this.handleChecked.bind(this);
        this.handleChange = this.handleChange.bind(this);

        this.getAllMilestones = this.getAllMilestones.bind(this);
        this.addCustomMilestone = this.addCustomMilestone.bind(this);
        this.addMilestone = this.addMilestone.bind(this);

        this.validate = this.validate.bind(this);
    }

    validate() {
        var description = this.state.description
        if (description === '' && !description == /^\S{3,}$/) {
            return 'error';
        }
        else {
            return "success";
        }
    }

    handleChange(e) {
        var regex = "^\\s+$";
        if (e.target.value !== '' && !e.target.value.match(regex)) {
            this.setState({
                [e.target.name]: e.target.value,
                validate: "milestone is valid"

            })
        }
        else {
            this.setState({
                [e.target.name]: e.target.value,
                validate: "please enter valid milestone"
            })
        }
    }

    handleClose() {
        this.setState({
            show: false,
            checkedList: []
        });
    }

    handleShow() {
        this.setState({ show: true });
    }

    handleInputOpen() {
        this.setState({ showInput: true });
    }

    handleInputClose() {
        this.setState({ showInput: false });
    }

    handleChecked(e) {
        var submitList = [...this.state.checkedList];
        var index = submitList.indexOf(e.target.name);

        if (e.target.checked) {
            submitList.push(e.target.name);
        } else {
            submitList.splice(index, 1);
        }
        this.setState({ checkedList: submitList })
    }

    addMilestone() {
        for (var i = 0; i < this.state.checkedList.length; i++) {
            const submitData = {
                userId: this.props.userId,
                mentorId: this.props.mentorId,
                milestoneId: parseInt(this.state.checkedList[i]),
                isCompleted: false
            }
            milestoneService.createUserMilestone(submitData)
                .then(() => this.setState({ checkedList: [] }))
                .then(() => this.props.getMilestoneByUserId())
                .catch(console.error)
        }
        this.handleClose()
    }

    componentDidMount() {
        this.getAllMilestones();
    }

    getAllMilestones() {
        milestoneService.readAllMilestones()
            .then(response => {
                this.setState({
                    milestones: response.items
                })
            })
    }

    addCustomMilestone() {
        if (this.state.validate == "milestone is valid") {
            const submitData = {
                description: this.state.description,
                customMentorId: this.props.mentorId,
                isCustomCreateByMentor: 1
            };
            milestoneService.createMilestone(submitData)
                .then(() => this.getAllMilestones())
                .then(() => this.handleInputClose())
                .catch(console.error);
        }
    }

    render() {
        const modalStyle = {
            position: 'fixed',
            zIndex: 1040,
            top: 0, bottom: 0, left: 0, right: 0
        };

        const backdropStyle = {
            ...modalStyle,
            zIndex: 'auto',
            backgroundColor: '#000',
            opacity: 0.5
        }

        const milestoneList = this.state.milestones
            .map(milestone => {
                if (!milestone.isCustomCreateByMentor) {
                    return (
                        <tr key={milestone.id} style={{ verticalAlign: 'middle', textAlign: 'center' }}>
                            <td > {milestone.id}</td>
                            <td> {milestone.description}</td>
                            <td>
                                <Checkbox
                                    style={{ display: 'flex', paddingTop: '14px', justifyContent: 'center' }}

                                    name={milestone.id}
                                    defaultChecked={false}
                                    onChange={this.handleChecked} />
                            </td>
                        </tr>
                    )
                }
            })

        const customMilestoneList = this.state.milestones
            .map(milestone => {
                if (milestone.isCustomCreateByMentor && milestone.customMentorId === this.props.mentorId) {
                    return (
                        <tr key={milestone.id} style={{ verticalAlign: 'middle', textAlign: 'center' }}>
                            <td > {milestone.id}</td>
                            <td> {milestone.description}</td>
                            <td>
                                <Checkbox

                                    style={{ display: 'flex', paddingTop: '14px', justifyContent: 'center' }}
                                    name={milestone.id}
                                    defaultChecked={false}
                                    onChange={this.handleChecked} />
                            </td>
                        </tr>
                    )
                }
            })



        return (
            <div>
                <span onClick={e => this.setState({ show: true })}>
                    <button style={{ textAlign: 'right' }} className="btn btn-light"> <div><i class="zmdi zmdi-plus zmdi-hc-fw"></i>Add Milestone</div></button>
                </span>

                <Modal 
                    show={this.state.show} 
                    onHide={this.handleClose} 
                    animation={false} 
                    backdropStyle={backdropStyle}>

                    <Modal.Header >
                        <div className='col-sm-12'>
                            <button className="btn btn-light float-right" style={{ backgroundColor: '#264057' }} onClick={this.handleClose}>x</button>
                        </div>
                    </Modal.Header>
                        <Modal.Title style={{ textAlign: 'center' }}>Milestone List</Modal.Title>

                    <Modal.Body>
                        <table className="table table-inverse">
                            <thead>
                                <tr >
                                    <th>Milestone Id</th><th style={{ display: 'flex', justifyContent: 'center' }}>Description</th><th>Check</th>
                                </tr>
                            </thead>
                            <tbody>
                                {milestoneList}
                                <td></td><h3 style={{ display: 'flex', justifyContent: 'center' }}>- Custom Milestones -</h3>
                                {customMilestoneList}
                            </tbody>
                        </table>


                    </Modal.Body>
                    <Modal.Footer>
                        <div className='col-sm-12 btn-demo'>
                        <Button className='btn btn-light' onClick={this.handleInputOpen}>Add Custom Milestone</Button>
                        <Button className='btn btn-light' onClick={this.addMilestone}>Submit</Button>
                        <Button className='btn btn-light' onClick={this.handleClose}>Cancel</Button>
                        </div>
                    </Modal.Footer>
                </Modal>

                <Modal show={this.state.showInput} onHide={this.handleInputClose} animation={false} backdropStyle={backdropStyle}>
                    <Modal.Header closeButton>
                        <Modal.Title>Custom Milestone</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <FormGroup validationState={this.validate()}
                        >
                            <FormControl
                                validationState={this.validate()}
                                componentClass='textarea'
                                style={{ width: 400, borderWidth: 1, height: 180 }} name="description" value={this.state.description} onChange={this.handleChange}>
                            </FormControl>
                        </FormGroup>
                        <HelpBlock>{this.state.validate}</HelpBlock>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button onClick={this.addCustomMilestone}>Submit</Button>
                        <Button onClick={this.handleInputClose}>Cancel</Button>
                    </Modal.Footer>
                </Modal>
            </div>
        )
    }
}

