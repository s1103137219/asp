using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;

namespace WebApplication3.Models
{
    public class OrderService
    {
        private string GetDBConnectionString()
        {
            return
                System.Configuration.ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString.ToString();
        }

        public Models.Order GetOrderByIdForUpdate(string orderId)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT 
					A.OrderID,A.CustomerID,B.CompanyName As CompanyName,
					A.EmployeeID,C.LastName+ C.FirstName As EmployeeName,
					A.OrderDate,A.RequiredDate,A.ShippedDate,
					A.ShipperID,D.CompanyName As ShipperName,A.Freight,
					A.ShipName,A.ShipAddress,A.ShipCity,A.ShipRegion,A.ShipPostalCode,A.ShipCountry
					From Sales.Orders As A 
					INNER JOIN Sales.Customers As B ON A.CustomerID=B.CustomerID
					INNER JOIN HR.Employees As C On A.EmployeeID=C.EmployeeID
					inner JOIN Sales.Shippers As D ON A.ShipperID=D.ShipperID
					Where  (A.OrderID=@OrderID)";


            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@OrderId", orderId));

                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            List<Models.OrderDetails> ordertail = UpdateOrderDetail(orderId);
            Models.Order order = new Order();
            foreach(DataRow row in dt.Rows)
            {
                order.OrderID=row["OrderID"].ToString();  
                order.CustomerID = (int)row["CustomerID"];
                order.EmployeeID = (int)row["EmployeeID"];
                order.Orderdate = row["Orderdate"].ToString();
                order.RequireDdate = row["RequiredDate"].ToString();
                order.ShippedDate = row["ShippedDate"].ToString();
                order.ShipperID = (int)row["ShipperID"];
                order.Freight = (decimal)row["Freight"];
                order.ShipCountry =row["ShipCountry"].ToString();
                order.ShipCity = row["ShipCity"].ToString();
                order.ShipRegion = row["ShipRegion"].ToString();
                order.ShipPostalCode= row["ShipPostalCode"].ToString();
                order.ShipAddress= row["ShipAddress"].ToString();
                order.ShipName= row["ShipName"].ToString();
                order.OrderDetails = ordertail;

            }


            return order;
           
        }

        public List<Models.Order> GetOrderById(string orderId)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT 
					A.OrderID,A.CustomerID,B.CompanyName As CompanyName,
					A.EmployeeID,C.LastName+ C.FirstName As EmployeeName,
					A.OrderDate,A.RequiredDate,A.ShippedDate,
					A.ShipperID,D.CompanyName As ShipperName,A.Freight,
					A.ShipName,A.ShipAddress,A.ShipCity,A.ShipRegion,A.ShipPostalCode,A.ShipCountry
					From Sales.Orders As A 
					INNER JOIN Sales.Customers As B ON A.CustomerID=B.CustomerID
					INNER JOIN HR.Employees As C On A.EmployeeID=C.EmployeeID
					inner JOIN Sales.Shippers As D ON A.ShipperID=D.ShipperID
					Where  (A.OrderID=@OrderID OR @OrderID='')";


            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@OrderId", orderId == null ? string.Empty : orderId));

                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapOrderDataToList(dt);
        }

        /// <summary>
		/// 依照條件取得訂單資料
		/// </summary>
		/// <returns></returns>
		public List<Models.Order> GetOrderByCondtioin(Models.OrderSearchArg arg)
        {

            DataTable dt = new DataTable();
            string sql = @"SELECT 
					A.OrderID,A.CustomerID,B.CompanyName As CompanyName,
					A.EmployeeID,C.LastName+ C.FirstName As EmployeeName,
					A.OrderDate,A.RequiredDate,A.ShippedDate,
					A.ShipperID,D.CompanyName As ShipperName,A.Freight,
					A.ShipName,A.ShipAddress,A.ShipCity,A.ShipRegion,A.ShipPostalCode,A.ShipCountry
					From Sales.Orders As A 
					INNER JOIN Sales.Customers As B ON A.CustomerID=B.CustomerID
					INNER JOIN HR.Employees As C On A.EmployeeID=C.EmployeeID
					inner JOIN Sales.Shippers As D ON A.ShipperID=D.ShipperID
					Where (A.OrderID=@OrderID or @OrderID='') AND (B.CompanyName Like @CompanyName Or @CompanyName='') and (C.EmployeeID = @EmployeeID Or @EmployeeID='') and (D.ShipperID = @ShipperID Or @ShipperID='') and (A.OrderDate=@Orderdate Or @Orderdate='') and (A.RequiredDate=@RequireDdate Or @RequireDdate='') and (A.ShippedDate=@ShippedDate Or @ShippedDate='')";


            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@CompanyName", arg.CompanyName== null ? string.Empty : '%'+arg.CompanyName+'%'));
                cmd.Parameters.Add(new SqlParameter("@Orderdate", arg.Orderdate == null ? string.Empty : arg.Orderdate));
                cmd.Parameters.Add(new SqlParameter("@RequireDdate", arg.RequireDdate == null ? string.Empty : arg.RequireDdate));
                cmd.Parameters.Add(new SqlParameter("@ShippedDate", arg.ShippedDate == null ? string.Empty : arg.ShippedDate));
                cmd.Parameters.Add(new SqlParameter("@EmployeeID", arg.EmployeeID == null ? string.Empty : arg.EmployeeID));
                cmd.Parameters.Add(new SqlParameter("@ShipperID", arg.ShipperID == null ? string.Empty : arg.ShipperID));
                cmd.Parameters.Add(new SqlParameter("@OrderID", arg.OrderID == null ? string.Empty : arg.OrderID));
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }


            return this.MapOrderDataToList(dt);
        }
        /// <summary>
        /// 依照OrderID刪除訂單(連動刪除)
        /// </summary>
        /// <param name="orderId"></param>
        public int DeleteOrderDetailById(string orderId)
        {
            try
            {
                string sql = "Delete FROM Sales.OrderDetails Where OrderID=@OrderID";
                using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.Add(new SqlParameter("@OrderID", orderId));
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                string sql2 = "Delete FROM Sales.Orders Where OrderID=@OrderID";
                using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql2, conn);
                    cmd.Parameters.Add(new SqlParameter("@OrderID", orderId));
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                return 0;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
		/// 新增訂單
		/// </summary>
		/// <param name="order"></param>
		/// <returns>訂單編號</returns>
		public int InsertOrder(Models.Order order)
        {
            string sql = @" Insert INTO Sales.Orders
						 (
							CustomerID,EmployeeID,Orderdate,RequireDdate,ShippedDate,ShipperID,Freight,
							ShipCountry,ShipCity,ShipRegion,ShipPostalCode,ShipAddress,ShipName
						)
						VALUES
						(
							@CustomerID,@EmployeeID,@Orderdate,@RequireDdate,@ShippedDate,@ShipperID,@Freight,
							@ShipCountry,@ShipCity,@ShipRegion,@ShipPostalCode,@ShipAddress,@ShipName
						)
                        Select SCOPE_IDENTITY() as orderid 						
						";
        
            int orderId;
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@CustomerID", order.CustomerID));
                cmd.Parameters.Add(new SqlParameter("@EmployeeID", order.EmployeeID));
                cmd.Parameters.Add(new SqlParameter("@Orderdate", order.Orderdate));
                cmd.Parameters.Add(new SqlParameter("@RequireDdate", order.RequireDdate));
                cmd.Parameters.Add(new SqlParameter("@ShippedDate", order.ShippedDate));
                cmd.Parameters.Add(new SqlParameter("@ShipperID", order.ShipperID));
                cmd.Parameters.Add(new SqlParameter("@Freight", order.Freight));
                cmd.Parameters.Add(new SqlParameter("@ShipCountry", order.ShipCountry));
                cmd.Parameters.Add(new SqlParameter("@ShipCity", order.ShipCity));
                cmd.Parameters.Add(new SqlParameter("@ShipRegion", order.ShipRegion));
                cmd.Parameters.Add(new SqlParameter("@ShipPostalCode", order.ShipPostalCode));
                cmd.Parameters.Add(new SqlParameter("@ShipAddress", order.ShipAddress));
                cmd.Parameters.Add(new SqlParameter("@ShipName", order.ShipName));

                orderId = Convert.ToInt32(cmd.ExecuteScalar());
                conn.Close();
            }
            string sql2 = @" Insert INTO Sales.OrderDetails
						 (
							OrderID,ProductID,UnitPrice,Qty
						)
						VALUES
						(
							@OrderID,@ProductID,@UnitPrice,@Qty
						)						
						";
            
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                
                for (int i = 0; i < order.OrderDetails.Count; i++)
                {
                SqlCommand cmd = new SqlCommand(sql2, conn);
                cmd.Parameters.Add(new SqlParameter("@OrderID", orderId));
                cmd.Parameters.Add(new SqlParameter("@ProductID",order.OrderDetails[i].ProductID));
                cmd.Parameters.Add(new SqlParameter("@UnitPrice", order.OrderDetails[i].UnitPrice));
                cmd.Parameters.Add(new SqlParameter("@Qty", order.OrderDetails[i].Qty));
                cmd.ExecuteScalar();
                }
                conn.Close();
            }
            return orderId;

        }
        /// <summary>
		/// 修改訂單上半部
		/// </summary>
		/// <param name="order"></param>
        public void UpdateOrder(Models.Order order)
        {
            string sql = @" Update Sales.Orders SET
                            CustomerID=@CustomerID,EmployeeID=@EmployeeID,Orderdate=@Orderdate,RequireDdate=@RequireDdate,ShippedDate=@ShippedDate,ShipperID=@ShipperID,Freight=@Freight,
							ShipCountry=@ShipCountry,ShipCity=@ShipCity,ShipRegion=@ShipRegion,ShipPostalCode=@ShipPostalCode,ShipAddress=@ShipAddress,ShipName=@ShipName
                        where OrderID=@OrderID			
						";
         
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@CustomerID", order.CustomerID));
                cmd.Parameters.Add(new SqlParameter("@EmployeeID", order.EmployeeID));
                cmd.Parameters.Add(new SqlParameter("@Orderdate", order.Orderdate));
                cmd.Parameters.Add(new SqlParameter("@RequireDdate", order.RequireDdate));
                cmd.Parameters.Add(new SqlParameter("@ShippedDate", order.ShippedDate));
                cmd.Parameters.Add(new SqlParameter("@ShipperID", order.ShipperID));
                cmd.Parameters.Add(new SqlParameter("@Freight", order.Freight));
                cmd.Parameters.Add(new SqlParameter("@ShipCountry", order.ShipCountry));
                cmd.Parameters.Add(new SqlParameter("@ShipCity", order.ShipCity));
                cmd.Parameters.Add(new SqlParameter("@ShipRegion", order.ShipRegion));
                cmd.Parameters.Add(new SqlParameter("@ShipPostalCode", order.ShipPostalCode));
                cmd.Parameters.Add(new SqlParameter("@ShipAddress", order.ShipAddress));
                cmd.Parameters.Add(new SqlParameter("@ShipName", order.ShipName));
                cmd.Parameters.Add(new SqlParameter("@OrderID",order.OrderID));

                cmd.ExecuteScalar();
                conn.Close();
            }
        }
        /// <summary>
        /// 修改訂單下半部
        /// </summary>
        /// <param name="order"></param>
        public void UpdateOrdertwo(Models.Order order)
        {
            string sql = @"Delete FROM Sales.OrderDetails Where OrderID=@OrderID";	

            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);               
                cmd.Parameters.Add(new SqlParameter("@OrderID", order.OrderID));

                cmd.ExecuteScalar();
                conn.Close();
            }
            string sql2 = @" Insert INTO Sales.OrderDetails
						 (
							OrderID,ProductID,UnitPrice,Qty
						)
						VALUES
						(
							@OrderID,@ProductID,@UnitPrice,@Qty
						)						
						";

            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();

                for (int i = 0; i < order.OrderDetails.Count; i++)
                {
                    SqlCommand cmd = new SqlCommand(sql2, conn);
                    cmd.Parameters.Add(new SqlParameter("@OrderID", order.OrderID));
                    cmd.Parameters.Add(new SqlParameter("@ProductID", order.OrderDetails[i].ProductID));
                    cmd.Parameters.Add(new SqlParameter("@UnitPrice", order.OrderDetails[i].UnitPrice));
                    cmd.Parameters.Add(new SqlParameter("@Qty", order.OrderDetails[i].Qty));
                    cmd.ExecuteScalar();
                }
                conn.Close();
            }

        }
        /// <summary>
        /// 查詢訂單明細
        /// </summary>
        /// <param name="order"></param>
        public List<Models.OrderDetails> UpdateOrderDetail(string orderId)
        {
            DataTable dt = new DataTable();
            string sql = @"select * from Sales.OrderDetails where OrderID=@OrderID";


            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@OrderId", orderId));

                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return MapOrderDetailToList(dt);
            
        }
        private List<Models.OrderDetails> MapOrderDetailToList(DataTable orderData)
        {
            List<Models.OrderDetails> result = new List<OrderDetails>();


            foreach (DataRow row in orderData.Rows)
            {
                result.Add(new OrderDetails()
                {
                    ProductID = Convert.ToInt32(row["ProductID"]),
                    UnitPrice = Convert.ToInt32(row["UnitPrice"]),
                    Qty = Convert.ToInt32(row["Qty"]),
                    OrderID = Convert.ToInt32(row["OrderID"])

                });
            }
        return result;
            }


        private List<Models.Order> MapOrderDataToList(DataTable orderData)
        {
            List<Models.Order> result = new List<Order>();


            foreach (DataRow row in orderData.Rows)
            {
                result.Add(new Order()
                {
                    CustomerID = (int)row["CustomerID"],
                    CompanyName = row["CompanyName"].ToString(),
                    EmployeeID = (int)row["EmployeeID"],
                    EmployeeName=row["EmployeeName"].ToString(),
                    Freight = (decimal)row["Freight"],
                    Orderdate = row["Orderdate"].ToString(),
                    OrderID = row["OrderID"].ToString(),
                    RequireDdate = row["RequireDdate"].ToString(),
                    ShipAddress = row["ShipAddress"].ToString(),
                    ShipCity = row["ShipCity"].ToString(),
                    ShipCountry = row["ShipCountry"].ToString(),
                    ShipName = row["ShipName"].ToString(),
                    ShippedDate = row["ShippedDate"].ToString(),
                    ShipperID = (int)row["ShipperID"],
                    ShipperName = row["ShipperName"].ToString(),
                    ShipPostalCode = row["ShipPostalCode"].ToString(),
                    ShipRegion = row["ShipRegion"].ToString()
                    
                }
                );
            }
            return result;
        }



    }
}