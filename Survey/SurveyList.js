import React, { Component } from "react";
import * as surveyService from "../../../services/surveyService";
import swal from "sweetalert2";
import * as moment from "moment";
import { DropdownButton, MenuItem } from "react-bootstrap";

export default class SurveyList extends Component {
  constructor(props) {
    super(props);

    this.state = {
      receivedData: [],

      status: "active",
      index: 0,
      size: 12,
      totalCount: "",
      pageNumArr: []
    };

    this.handleChange = this.handleChange.bind(this);
    this.updateById = this.updateById.bind(this);
    this.readAll = this.readAll.bind(this);

    this.onDelete = this.onDelete.bind(this);
    this.addNewSurvey = this.addNewSurvey.bind(this);
    this.getByIndexAndSize = this.getByIndexAndSize.bind(this);

    //page navigation
    this.nextPage = this.nextPage.bind(this);
    this.prevPage = this.prevPage.bind(this);
    this.firstPage = this.firstPage.bind(this);
    this.lastPage = this.lastPage.bind(this);
    this.pageNumbers = this.pageNumbers.bind(this);
    this.pageSize = this.pageSize.bind(this);
  }

  handleChange(e) {
    this.setState({ [e.target.name]: e.target.value });
    console.log(e.target.value);
  }

  componentDidMount() {
    this.getByIndexAndSize(this.state.index, this.state.size);
    // console.log(this.state.pageNumArr)
  }

  getByIndexAndSize(index, size) {
    surveyService
      .getByIndSize(index, size)

      .then(response => {
        const dataToList = response.items;

        if (dataToList.length === 0) {
          dataToList[0].totalCount = 0;
        }

        const lastPage =
          Math.ceil(dataToList[0].totalCount / this.state.size) - 1;
        //console.log("page count "+lastPage);

        var pageArr = [];
        for (var i = 0; i < lastPage; i++) {
          pageArr.push(i);
        }
        this.setState({
          receivedData: dataToList,
          totalCount: lastPage,
          pageNumArr: pageArr
        });
      })
      .catch(console.log.error);
  }

  readAll() {
    surveyService
      .getByIndSize(this.state.index, this.state.size)
      .then(response => {
        const dataToList = response.items;

        this.setState({ receivedData: dataToList });
      })
      .catch(console.log.error);
  }

  updateById(e, id) {
    this.props.history.push("/survey-builder/" + id);
  }

  addNewSurvey() {
    this.props.history.push("/survey-builder/");
  }

  onDelete(e, id) {
    swal({
      title: "Are you sure you want to delete this survey?",
      text: "You won't be able to recover this!",
      type: "warning",
      showCancelButton: true,
      confirmButtonColor: "red",
      cancelButtonColor: '#7ac7f6',
      confirmButtonText: "Yes, delete it!",
      cancelButtonText: "No, keep it!",
      background: '#0f2940'
    }).then(result => {
      if (result.value) {
        surveyService.deleteById(id).then(this.readAll);
        swal({
          title: "Deleted!",
          text: "Survey has been deleted.",
          type: "success",
          background: '#0f2940',
          confirmButtonColor: '#7ac7f6'
        });
      }
    });
  }

  firstPage() {
    this.setState({ index: 0 });
    this.getByIndexAndSize(0, this.state.size);
  }

  prevPage() {
    if (this.state.index > 0) {
      var minusOne = parseInt(this.state.index, 10) - 1;
      this.setState({ index: minusOne });
      this.getByIndexAndSize(minusOne, this.state.size);
    }
  }

  nextPage() {
    if (this.state.index < this.state.totalCount) {
      var plusOne = parseInt(this.state.index, 10) + 1;
      this.setState({ index: plusOne });
      this.getByIndexAndSize(plusOne, this.state.size);
    }
  }

  lastPage() {
    this.setState({ index: this.state.totalCount });
    this.getByIndexAndSize(this.state.totalCount, this.state.size);
  }

  pageNumbers(e, index) {
    this.setState({ index: index });
    this.getByIndexAndSize(index, this.state.size);
    //console.log("checkind "+ JSON.stringify(index ))
  }

  pageSize(e) {
    this.handleChange(e);
    this.getByIndexAndSize(this.state.index, e.target.value);
  }

  render() {
    function status(id) {
      if (id === 2) {
        return <span className="issue-tracker__tag bg-orange"> draft</span>;
      } else if (id === 1) {
        return <span className="issue-tracker__tag bg-green"> active</span>;
      } else {
        return <span className="issue-tracker__tag bg-red"> retired</span>;
      }
    }

    const overflowSettings = {
      whiteSpace: "nowrap",
      maxWidth: "160px",
      overflow: "hidden",
      textOverflow: "ellipsis"
    };

    //pagination numbering
    var listPages = this.state.pageNumArr.map(item => {
      var pageNum = item + 1;
      return (
        <span>
          <li
            id={pageNum}
            onClick={e => {
              this.pageNumbers(e, pageNum);
            }}
            class="page-item"
          >
            <a class="page-link" href="#">
              {pageNum}
            </a>
          </li>
        </span>
      );
    });

    var listData = this.state.receivedData.map((item, index) => {
      return (
        <tr data-id={item.id}>
          <th scope="row">{index + 1}</th>
          <td>{item.id}</td>
          <td> {item.surveyParentId} </td>
          <td> {item.name}</td>
          <td style={overflowSettings}> {item.description} </td>
          <td> {item.version} </td>

          <td> {item.statusId} </td>
          <td> {item.ownerId} </td>
          <td> {item.typeId} </td>
          <td>{status(item.statusId)}</td>
          <td>
            {moment(item.dateCreated)
              .utc()
              .format("MM-DD-YYYY")}
          </td>
          <td>
            {moment(item.dateModified)
              .utc()
              .format("MM-DD-YYYY")}
          </td>

          <span>
            <button
              style={{ marginRight: "3px" }}
              id={item.id}
              onClick={e => this.updateById(e, item.id)}
              className="btn btn-light btn--icon"
              toggle="tooltip"
              placement="right"
              tooltip="edit"
              aria-describedby="tooltip"
            >
              <i className="zmdi zmdi-edit zmdi-hc-fw" />
            </button>
            <button
              id={item.id}
              onClick={e => this.onDelete(e, item.id)}
              className="btn btn-light btn--icon"
              toggle="tooltip"
              placement="right"
              data-original-title="delete"
              aria-describedby="tooltip"
            >
              <i className="zmdi zmdi-delete zmdi-hc-fw" />
            </button>
          </span>
        </tr>
      );
    });
    return (
      <React.Fragment>
        <header className="content__title">
          <h1>Survey List</h1>

          <div className="actions">
            <a
              className="actions__item zmdi zmdi-plus-circle"
              onClick={this.addNewSurvey}
            />
          </div>
        </header>
        {/* <div className="card-body"> */}

        <table className="table table-inverse">
          <thead>
            <tr>
              <th>Count</th>
              <th>Id</th>
              <th>ParentId</th>
              <th>Name</th>
              <th>Description</th>
              <th>Version</th>
              <th>StatusId</th>
              <th>OwnerId</th>
              <th>TypeId</th>
              <th>Status</th>
              <th>Created</th>
              <th>Modified</th>
              <th>Actions</th>
            </tr>
          </thead>

          <tbody>{listData}</tbody>
        </table>
        {/* </div> */}
        {/* <select className="form-control" name="size" value={this.state.size} onChange={this.pageSize}
                    style={{ width: 'auto', position: 'center' }}
                >
                    <option style={{ backgroundColor: "black" }} value='5'>5</option>
                    <option style={{ backgroundColor: "black" }} value='10'>10</option>
                    <option style={{ backgroundColor: "black" }} value='20'>20</option>
                    <option style={{ backgroundColor: "black" }} value='50'>50</option>
                </select> */}
        <ul className="pagination justify-content-center">
          <li
            id="first"
            onClick={this.firstPage}
            className="page-item pagination-first"
          >
            <a className="page-link" href="#" />
          </li>
          <li
            id="prev"
            onClick={this.prevPage}
            className="page-item pagination-prev"
          >
            <a className="page-link" href="#" />
          </li>
          {listPages}
          <li
            id="next"
            onClick={this.nextPage}
            className="page-item pagination-next"
          >
            <a className="page-link" href="#" />
          </li>
          <li
            id="last"
            onClick={this.lastPage}
            className="page-item pagination-last"
          >
            <a className="page-link" href="#" />
          </li>
        </ul>
      </React.Fragment>
    );
  }
}
