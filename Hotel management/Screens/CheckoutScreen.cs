﻿using Hotel_Management_System.Screens;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hotel_Management_System.Controllers
{
    public partial class CheckoutScreen : Form
    {

        DatabaseConnection dc = new DatabaseConnection();
        String query;

        public CheckoutScreen()
        {
            InitializeComponent();
            paymentIdField.ReadOnly = false;
            checkIfEmployee();
        }

        private void checkIfEmployee()
        {
            if (Statics.employeeIdTKN.Equals(0))
            {
                payButton.Enabled = true;
            }
        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            fetchData(0);
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            clearFields();
        }

        private void guna2CircleButton2_Click(object sender, EventArgs e)
        {
            Dashboard d = new Dashboard();
            d.loadForm(new HotelIntroScreen());
        }

        private void populateTable()
        {
            SqlConnection con = dc.getConnection();
            con.Open();
            String query = "SELECT PaymentId AS ID, PaymentStatus AS Status, PaymentType AS TYPE, PaymentAmount AS Amount, BookingId FROM Bookings.Payments WHERE BookingId IN (SELECT BookingId FROM Bookings.Booking WHERE HotelId = " + Statics.hotelIdTKN + ")";
            SqlDataAdapter sda = new SqlDataAdapter(query, con);
            SqlCommandBuilder builder = new SqlCommandBuilder(sda);
            var ds = new DataSet();
            sda.Fill(ds);
            checkoutTable.DataSource = ds.Tables[0];
            con.Close();
        }

        private void clearFields()
        {
            paymentIdField.Text = "";
            bookingIdCMBox.Text = "";
            paymentTypeCmbox.SelectedIndex = -1;
            amountField.Text = "";
        }

        private void populateBookingIdCmbox()
        {
            SqlConnection con = dc.getConnection();
            con.Open();
            query = "SELECT BookingId FROM Bookings.Booking WHERE Status = 'Checkin' AND HotelId = " + Statics.hotelIdTKN;
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                bookingIdCMBox.Items.Add(dr["BookingId"]);
            }
            con.Close();
        }


        private void CheckoutScreen_Load(object sender, EventArgs e)
        {
            populateTable();
            populateBookingIdCmbox();
        }

        private void bookingIdCMBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = int.Parse(bookingIdCMBox.Text);
            SqlConnection con = dc.getConnection();
            con.Open();
            query = "SELECT BookingAmount From Bookings.Booking WHERE BookingId = " + id;
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                amountField.Text = dr.GetSqlInt32(0).ToString();
            }
        }

        private void payButton_Click(object sender, EventArgs e)
        {
            if (bookingIdCMBox.SelectedIndex != -1 && paymentTypeCmbox.SelectedIndex != -1 && amountField.Text != "")
            {
                int bId = int.Parse(bookingIdCMBox.Text);
                int gId = getGuestIdS(bId);
                query = "UPDATE Hotels.Guests SET Status = 'Not Reserved' WHERE GuestId = " + gId;
                dc.setData(query, "");
                int a = getRoomId(bId);
                query = "UPDATE Rooms.Room SET Available = 'Yes' WHERE RoomId = " + a;
                dc.setData(query, "");
                delServiceUsed(bId);
                query = "INSERT INTO Bookings.Payments (PaymentStatus, PaymentType, PaymentAmount, BookingId, HotelId) VALUES ('"
        + statusField.Text + "', '"
        + paymentTypeCmbox.Text + "', "
        + amountField.Text + ", "
        + bookingIdCMBox.Text + ", "
        + Statics.hotelIdTKN + ")";
                dc.setData(query, "Checkout Data inserted successfully!");

                changeBookingStatus(bId);
                clearFields();
                bookingIdCMBox.Items.Clear();
                populateBookingIdCmbox();
                populateTable();
            }
            else
            {
                MessageBox.Show("All fields must be filled.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void changeBookingStatus(int bid)
        {
            query = "UPDATE Bookings.Booking SET Status = 'Checkout' WHERE BookingId = " + bid;
            dc.setData(query, "");
        }

        private void delServiceUsed(int id)
        {
            query = "DELETE FROM HotelService.ServicesUsed WHERE BookingId = " + id;
            dc.setData(query, "");
        }

        private int getRoomId(int bid)
        {
            SqlConnection con = dc.getConnection();
            con.Open();
            query = "SELECT RoomId from Rooms.RoomBooked WHERE BookingId = " + bid;

            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader dr = cmd.ExecuteReader();
            int id = 0;
            while (dr.Read())
            {
                id = dr.GetInt32(0);
            }
            return id;
        }

        private int getGuestIdS(int bid)
        {
            SqlConnection con = dc.getConnection();
            con.Open();
            query = "SELECT GuestId from Bookings.Booking WHERE BookingId = " + bid;
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader dr = cmd.ExecuteReader();
            int id = 0;
            while (dr.Read())
            {
                id = dr.GetInt32(0);
            }
            return id;
        }

        private void checkoutTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            populateTable();
            fetchData(1);
        }

        private void fetchData(int i)
        {
            String pId;
            if (i == 1)
            {
                if (checkoutTable.SelectedRows.Count > 0)
                    pId = checkoutTable.SelectedRows[0].Cells[0].Value.ToString();
                else
                {
                    MessageBox.Show("No row selected in the table.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                pId = paymentIdField.Text;
            }

            if (string.IsNullOrEmpty(pId))
            {
                MessageBox.Show("Please enter ID to search for a record.", "Missing Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool temp = false;
            using (SqlConnection con = dc.getConnection())
            {
                con.Open();
                query = "SELECT * FROM Bookings.Payments WHERE PaymentId = @PaymentId";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@PaymentId", pId);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            paymentIdField.Text = dr.GetInt32(0).ToString(); // PaymentId
                            statusField.Text = dr.IsDBNull(1) ? string.Empty : dr.GetString(1); // PaymentStatus
                            paymentTypeCmbox.Text = dr.IsDBNull(2) ? string.Empty : dr.GetString(2); // PaymentType
                            amountField.Text = dr.IsDBNull(3) ? "0.00" : dr.GetDecimal(3).ToString("F2"); // PaymentAmount
                            bookingIdCMBox.Text = dr.IsDBNull(4) ? string.Empty : dr.GetInt32(4).ToString(); // BookingId
                            temp = true;
                        }
                    }
                }
            }

            if (!temp && i == 0)
                MessageBox.Show("No record found.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


    }
}
