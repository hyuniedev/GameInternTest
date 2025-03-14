# Video thực hiện:
- Code Test: https://youtu.be/EEqPCP258PA
- Unity Test: https://youtu.be/tdNkOpuM1iM (Tính thêm 1 phần thời gian đọc và hiểu code có sẵn)
# Task 1: Re-Skin
- Tìm Prefabs của các đối tượng trong Resource. Thay đổi Sprite của từng Prefabs bằng đối các sprite có sẵn theo yêu cầu
# Task 2: Change game play
## 1. Di chuyển items từ bảng vào dòng bên dưới bằng cách nhấn
- Tạo 1 dòng các cells bên dưới gồm 5 cells (Gọi tạm là **Box**)
- Chuyển cơ chế kéo thành ấn -> Kiểm tra nếu có Item tại vị trí nhấn thì chuyển vào Box.
## 2. Không thể di chuyển lại từ Box về Board 
- Thêm trường vào class Cell để đánh dấu xem cell nằm trong Board hay Box. Tránh việc Nhấn vào Item trong Box
- Khi nhấn sẽ kiểm tra, nếu là cell trong Board thì di chuyển vào trong Box
## 3. Xóa 3 items trong Box nếu cùng loại
- Khi thêm 1 Item vào trong box, kiểm tra xem trong box có bao nhiêu item cùng loại đó. nếu có 3 item thì sẽ xóa 3 item đó đi
- Dồn lại Box khi xóa các item trùng nhau.
- Sắp xếp cho các item trùng nhau luôn ở cạnh nhau
## 4. Kiểm tra điều kiện thắng
- Sau khi thêm 1 item vào Box, kiểm tra xem trong board còn item nào không. Nếu không còn thì sẽ thắng.
- Hiển thị UI khi thắng.
## 5. Kiểm tra điều kiện thua
- Sau khi thêm 1 item vào Box, nếu box đã đủ 5 item (nhiều nhất 2 item cùng loại) thì sẽ thua
- Hiển thị UI khi thua.
## In Requirements
## 1. Số Item trong bảng ban đầu luôn chi hết cho 3
- Đảm bảo số cell trong bảng chia hết cho 3.
## 2. Vùng bên dưới có 5 Cells
- Fix 5 ô khi tạo Box
## 3. Hiển thị màn hình thắng khi hoàn thành trò chơi
- Hiển thị Panel UI thắng đơn giản khi hoàn thành
## 4. Hiển thị màn hình thua khi thất bại
- Hiển thị Panel UI thua đơn giản khi thất bại
## 5. Tạo nút Autoplay
- Khi nhấn vào nút, thực hiện chạy Coroutine sau mỗi 0.5s
- Duyệt qua tất cả các item trong Board, chọn 3 Item giống nhau và đưa vào Box
## 6. Tạo nút Auto Lose
- Khi nhấn vào nút, thực hiện chạy Coroutine sau mỗi 0.5s
- Duyệt qua tất cả các item trong Board, đảm bảo chọn nhiều nhất 2 item giống nhau cho vào board.
# Task 3: Cải thiện gameplay
## 1. Đảm bảo bảng ban đầu chứa tất cả các loại cá.
## 2. Thêm animation cho item khi thay đổi cell, và thêm animation cho item khi bị xóa trong box
- Update hàm AnimationMoveToPosition -> tăng thời gian di chuyển lên 0.5f để thấy rõ hơn.
- Dùng DOTween trong hàm Clear, thêm animation cho item khi bị xóa. Sau khi xong animation sẽ xóa item (Gọi hàm OnComplete trong DOTween)
## 3. Thêm Time Attack Mode
- Thêm Button ngoài Menu, khi ấn vào thực hiện chuyển vào chế độ Time Attack
- Trong hàm Update của BoardController, Kiểm tra xem đang ở chế độ nào, nếu ở chế độ Time Attack thì bỏ qua cơ chế thua khi đầy Box
- Kiểm tra xem Item trong cell được chọn là trong board hay box. nếu trong box thì di chuyển ngược lại về vị trí ban đầu trong board, nếu trong board thì di chuyển về box như bình thường
- trong class item, thêm 1 thuộc tính initCell để lưu lại cell ban đầu, để giúp cho item di chuyển ngược lại.
- Chạy coroutine, tính thời gian trong 1 phút. sau 1 phút sẽ thua.
