**Thông tin nền:**

**Ngày:** 2004-08-17  
**Triệu chứng:** Vòng lặp vô tận khi giải mã tín hiệu Q.931.  
**Nguyên nhân:** Khi tìm thấy id của một thành phần chưa biết trong tín hiệu Q.931, ta tìm cách bỏ qua nó bằng cách lấy chiều dài, và di chuyển con trỏ pos tương ứng với độ dài tìm được. Tuy nhiên, với trường hợp độ dài bằng 0 làm ta liên tục bỏ qua cùng 1 id.  
**Cách tìm ra:** Nhờ vào phân tích tín hiệu SETUP lấy từ trace của Ethereal ở Nortel. Tín hiệu của họ có độ dài 1016 bytes, nhưng MSX_MAX_LEN chỉ có 1000. Bình thường ta sẽ nhận một tín hiệu bị cắt từ common/Communication.cxx, nhưng ở đây khi cung cấp dữ liệu trực tiếp để phân tích, khoảng bộ nhớ vượt quá array bị truy cập, và vô tình nó bằng 0, làm xuất hiện lỗi. Để sửa lỗi, tôi đã thêm vào vài lệnh print trong phần code phân tích Q.931. Nhưng may mắn là dữ liệu lại bằng 0.
Cách sửa – Quá trình sửa:

**Sửa:** Nếu chiều dài tìm thấy bằng 0, đặt nó lại bằng 1. Như vậy chúng ta sẽ luôn đi tiếp được.  
**Sửa trong file(s):** callh/q931_msg.cxx  
**Thủ phạm là tôi:** Đúng vậy.  
**Thời gian sửa bug:** 1 giờ.   

**Bài học rút ra được:** Đặt “niềm tin lầm chỗ” vào dữ liệu của tín hiệu gửi tới. Giá trị dữ liệu có thể quá lớn làm chương trình chạy sai. Ngoài ra khi chiều dài bằng 0 cũng có thể là một dấu hiệu xấu.